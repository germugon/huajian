using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitLookup.AICheck.Entity;
using RevitLookup.AICheck.Result;
using Form = System.Windows.Forms.Form;

namespace RevitLookup.AICheck.Forms
{
	public partial class CheckRvt : Form
	{
		private readonly ExternalCommandData _cmdData;
		private LinkedList<string> files = new LinkedList<string>();
		//store in db
		private Dictionary<string, string[]> buildingTypeDict = new Dictionary<string, string[]>()
		{
			{ "公共建筑", new string[] { "医疗", "交通", "科教", "商业", "文体" } },
			{ "居住建筑", new string[] { "住宅", "宿舍", "招待所" } },
			{ "工业建筑", new string[] { "厂房"} },
			{ "农业建筑", new string[] { "种子库"} }
		};

		private string[] buildingLevelDict = new string[] { "低层", "多层", "高层", "超层" };

		public CheckRvt(ExternalCommandData cmdData)
		{
			TextWriterTraceListener listener = new TextWriterTraceListener(Console.Out);
			Trace.Listeners.Add(listener);

			InitializeComponent();

			cbxBuildingType1.Items.AddRange(buildingTypeDict.Keys.ToArray());
			cbxBuildingType1.SelectedIndex = 0;
			cbxBuildingType2.Items.AddRange(buildingTypeDict.ElementAt(0).Value);
			cbxBuildingType2.SelectedIndex = 0;
			cbxBuildingLevel.Items.AddRange(buildingLevelDict);
			cbxBuildingLevel.SelectedIndex = 0;

			_cmdData = cmdData;
		}


		private void cbxBuildingType1_SelectedIndexChanged(object sender, EventArgs e)
		{
			string buildingType1 = cbxBuildingType1.SelectedItem as string;
			string[] buildingType2;
			if (buildingTypeDict.TryGetValue(buildingType1, out buildingType2))
			{
				cbxBuildingType2.Items.Clear();
				cbxBuildingType2.Items.AddRange(buildingType2);
				cbxBuildingType2.SelectedIndex = 0;
			}
		}


		private void btnSelectFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "rvt files (*.rvt)|*.rvt";
			openFileDialog.FilterIndex = 2;
			openFileDialog.RestoreDirectory = true;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string path = openFileDialog.FileName;
				if (string.IsNullOrEmpty(path))
				{
					MessageBox.Show(@"Empty Path", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
				else
				{
					files.Clear();
					tbFolderPath.Text = "";
					tbFilePath.Text = path;
					files.AddLast(path);
				}
			}
		}

		private void btnSelectFolder_Click(object sender, System.EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				string folderPath = folderBrowserDialog.SelectedPath;
				if (string.IsNullOrEmpty(folderPath))
				{
					MessageBox.Show(@"Empty Path", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
				else
				{
					files.Clear();
					tbFolderPath.Text = "";
					tbFolderPath.Text = folderPath;
					findFile(folderPath, files);
				}
			}
		}

		private void findFile(string path, LinkedList<string> files)
		{
			foreach (string folder in Directory.GetDirectories(path))
			{
				Trace.WriteLine(folder);
				findFile(folder, files);
			}
			foreach (string file in Directory.GetFiles(path, "*.rvt"))
			{
				Trace.WriteLine(file);
				files.AddLast(file);
			}
		}

		private void readDoc(string path)
		{
			Document _doc = _cmdData.Application.Application.OpenDocumentFile(path);
			FilteredElementCollector elemTypeCtor = (new FilteredElementCollector(_doc)).WhereElementIsElementType();
			FilteredElementCollector notElemTypeCtor = (new FilteredElementCollector(_doc)).WhereElementIsNotElementType();
			FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith(notElemTypeCtor);

			foreach (Element element in allElementCtor)
			{

				if (element.Category != null && element.Category.Name != null &&
					(element.Category.Name.Contains("门") || element.Category.Name.Contains("楼梯")))
				{
					if (element.UniqueId != null)
					{
						ParameterSet paramset = _doc.GetElement(element.UniqueId).Parameters;
						foreach (Parameter param in paramset)
						{
							if (param.Definition != null && param.Definition.Name != null && param.Definition.Name.Contains("宽度"))
							{
								int val = 0;
								if (param.AsValueString() != null && Int32.TryParse(param.AsValueString(), out val))
								{
									if (val > 0)
									{
										Trace.WriteLine(element.Category.Name + "(" + element.UniqueId + "):" + param.Definition.Name + "=" + val);
									}
								}
							}
						}
					}
				}
			}
		}


		private void checkEgressDoor(string path, string debugFilePath)
		{
			Trace.WriteLine(path);
			Report.exportFile(debugFilePath, path);

			Document _doc = _cmdData.Application.Application.OpenDocumentFile(path);
			FilteredElementCollector collector = new FilteredElementCollector(_doc);

			ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
			FilteredElementCollector elements = collector.WherePasses(filter).OfCategory(BuiltInCategory.OST_Doors);
			foreach (Element elem in elements)
			{
				//Cast to FamilyInstance is safe because ElementClassFilter for FamilyInstance was used
				FamilyInstance familyInstance = elem as FamilyInstance;
				Element fromRoom = familyInstance.FromRoom;
				Element toRoom = familyInstance.ToRoom;

				Boolean isEgressDoor = Door.getEgressDoor(fromRoom, toRoom);
				if (isEgressDoor)
				{
					string elemInfo = elem.Name + "(" + elem.Id + "):" +
						(fromRoom == null ? "" : fromRoom.Name) + "(fromRoom)->" + (toRoom == null ? "" : toRoom.Name) + "(toRoom)";
					Trace.WriteLine(elemInfo);
					Report.exportFile(debugFilePath, elemInfo);
					foreach (Parameter param in _doc.GetElement(elem.GetTypeId()).Parameters)
					{
						if (param.Definition != null && param.Definition.Name != null && param.Definition.Name.Equals("宽度"))
						{
							object val = getParamValue(param, _doc);
							if (val != null)
							{
								string paramInfo = elem.Name + "(" + elem.Id + "):" + param.Definition.Name + "=" + val;
								Trace.WriteLine(paramInfo);
								Report.exportFile(debugFilePath, paramInfo);
								if ((double)val < 0.9)
								{
									Report.exportFile(debugFilePath, "Break");
								}
								else
								{
									Report.exportFile(debugFilePath, "Pass");
								}
							}
							break;
						}
					}
					Report.exportFile(debugFilePath, "");
				}
			}
			Report.exportFile(debugFilePath, "");
		}


		private void checkStair(string path, string debugFilePath)
		{
			Trace.WriteLine(path);
			Report.exportFile(debugFilePath, path);

			Document _doc = _cmdData.Application.Application.OpenDocumentFile(path);
			FilteredElementCollector collector = new FilteredElementCollector(_doc);

			ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
			FilteredElementCollector elements = collector.WherePasses(filter).OfCategory(BuiltInCategory.OST_Doors);
			foreach (Element elem in elements)
			{
				//Cast to FamilyInstance is safe because ElementClassFilter for FamilyInstance was used
				FamilyInstance familyInstance = elem as FamilyInstance;
				Element fromRoom = familyInstance.FromRoom;
				Element toRoom = familyInstance.ToRoom;

				Boolean isEgressDoor = Door.getEgressDoor(fromRoom, toRoom);
				if (isEgressDoor)
				{
					string elemInfo = elem.Name + "(" + elem.Id + "):" +
						(fromRoom == null ? "" : fromRoom.Name) + "(fromRoom)->" + (toRoom == null ? "" : toRoom.Name) + "(toRoom)";
					Trace.WriteLine(elemInfo);
					Report.exportFile(debugFilePath, elemInfo);
					foreach (Parameter param in _doc.GetElement(elem.GetTypeId()).Parameters)
					{
						if (param.Definition != null && param.Definition.Name != null && param.Definition.Name.Equals("宽度"))
						{
							object val = getParamValue(param, _doc);
							if (val != null)
							{
								string paramInfo = elem.Name + "(" + elem.Id + "):" + param.Definition.Name + "=" + val;
								Trace.WriteLine(paramInfo);
								Report.exportFile(debugFilePath, paramInfo);
								if ((double)val < 0.9)
								{
									Report.exportFile(debugFilePath, "Break");
								}
								else
								{
									Report.exportFile(debugFilePath, "Pass");
								}
							}
							break;
						}
					}
					Report.exportFile(debugFilePath, "");
				}
			}
			Report.exportFile(debugFilePath, "");
		}

		private object getParamValue(Parameter para, Document document)
		{
			object defValue = null;
			// Use different method to get parameter data according to the storage type
			switch (para.StorageType)
			{
				case StorageType.Double:
					//covert the number into Metric
					defValue = para.AsDouble();
					break;
				case StorageType.ElementId:
					//find out the name of the element
					Autodesk.Revit.DB.ElementId id = para.AsElementId();
					if (id.IntegerValue >= 0)
					{
						defValue = document.GetElement(id).Name;
					}
					else
					{
						defValue = id.IntegerValue;
					}
					break;
				case StorageType.Integer:
					if (ParameterType.YesNo == para.Definition.ParameterType)
					{
						if (para.AsInteger() == 0)
						{
							defValue = false;
						}
						else
						{
							defValue = true;
						}
					}
					else
					{
						defValue = para.AsInteger();
					}
					break;
				case StorageType.String:
					defValue = para.AsString();
					break;
				default:
					defValue = "Unexposed parameter.";
					break;
			}

			return defValue;
		}

		private void btnDumpCheckInfo_Click(object sender, System.EventArgs e)
		{
			string debugFilePath = "E:\\华建AI\\BIM(139.217.202.141)\\bim demo - 20180605\\report.txt";
			Report.clearFile(debugFilePath);

			Trace.WriteLine(tbBuildingName.Text.Trim());
			Trace.WriteLine(cbxBuildingType1.SelectedItem + "-" + cbxBuildingType2.SelectedItem
				+ "-" + cbxBuildingLevel.SelectedItem);
			Report.exportFile(debugFilePath, tbBuildingName.Text.Trim());	
			Report.exportFile(debugFilePath, cbxBuildingType1.SelectedItem + "-" + cbxBuildingType2.SelectedItem
				+ "-" + cbxBuildingLevel.SelectedItem);
			foreach (string path in files)
			{
				checkEgressDoor(path, debugFilePath);
			}

		}

	}
}
