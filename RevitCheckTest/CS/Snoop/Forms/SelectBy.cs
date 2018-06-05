using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace RevitLookup.Snoop.Forms
{
    public partial class SelectBy : Form
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

        public SelectBy(ExternalCommandData cmdData)
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


        private void readDocNew(string path)
        {
            Document _doc = _cmdData.Application.Application.OpenDocumentFile(path);
            FilteredElementCollector stairElem = (new FilteredElementCollector(_doc)).OfCategory(BuiltInCategory.OST_Stairs);

            foreach (Element element in stairElem)
            {

                if(element.UniqueId != null)
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

        private void btnDumpInfo_Click(object sender, System.EventArgs e)
        {
            Trace.WriteLine(tbBuildingName.Text.Trim());
            Trace.WriteLine(cbxBuildingType1.SelectedItem + "-" + cbxBuildingType2.SelectedItem
                + "-" + cbxBuildingLevel.SelectedItem);
            foreach (string path in files)
            {         
                Trace.WriteLine(path);
                readDocNew(path);
            }
        }

    }
}