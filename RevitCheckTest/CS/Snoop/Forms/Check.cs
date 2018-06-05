using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitLookup.Snoop.Forms
{
    

    public partial class Check : System.Windows.Forms.Form
    {
        private ExternalCommandData cmdData = null;
        private int lastID = -1;
        private ArrayList checkedList = new ArrayList();
        private Document doc = null;

        public Check(ArrayList objs, ExternalCommandData cmdData,Document doc)
        {
            this.cmdData = cmdData;
            this.doc = doc;
            InitializeComponent();
            CommonInit(objs);
        }

        private void CommonInit(ArrayList objs)
        {
            this.dataGridView1.DataSource = CreateSource(objs);
            this.dataGridView1.Columns[0].FillWeight = 100;
            this.dataGridView1.Update();
        }

        private DataTable CreateSource(ArrayList objs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", System.Type.GetType("System.Int64"));

            foreach (Element element in objs)
            {
                DataRow dr = dt.NewRow();
                dr[0] = element.Id.IntegerValue;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            this.textBox1.Text = "";

            string currentID_str = this.dataGridView1.CurrentCell.Value.ToString();
            int currentID = -1;

            UIApplication app = this.cmdData.Application;
            Document doc = app.ActiveUIDocument.Document;

            if (Int32.TryParse(currentID_str,out currentID))
            {
                ElementId ei = new ElementId(currentID);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)255, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetCutLineWeight(10);
                ogs.SetProjectionLineWeight(10);
                ogs.SetProjectionLineColor(color);
                ogs.SetProjectionFillColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetCutLineColor(color);
                
                ogs.SetProjectionFillPatternVisible(true);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(ei, ogs);
                trans.Commit();
                this.textBox1.Text = "ID "+currentID_str+" Break rule(s):";
                this.textBox1.Text += Environment.NewLine;
                this.textBox1.Text += Environment.NewLine;
                this.textBox1.Text += "  1. The width of a residential building door should not be less than 0.90m";
            }

            if (this.lastID != -1)
            {
                ElementId ei = new ElementId(this.lastID);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)0, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetProjectionFillColor(color);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(ei, ogs);
                trans.Commit();
            }

            if (Int32.TryParse(currentID_str, out currentID))
            {
                this.lastID = currentID;
                this.checkedList.Add(currentID);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UIApplication app = this.cmdData.Application;
            Document doc = app.ActiveUIDocument.Document;

            foreach (int id in this.checkedList)
            {
                ElementId ei = new ElementId(id);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)0, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetProjectionFillColor(color);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(ei, ogs);
                trans.Commit();
            }
            this.lastID = -1;
            this.checkedList = new ArrayList();

            this.Close();
        }

        private void Check_Load(object sender, EventArgs e)
        {

        }


        private void Check_FormClosing(object sender, FormClosingEventArgs e)
        {

            UIApplication app = this.cmdData.Application;
            Document doc = app.ActiveUIDocument.Document;

            foreach (int id in this.checkedList)
            {
                ElementId ei = new ElementId(id);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)0, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetProjectionFillColor(color);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(ei, ogs);
                trans.Commit();
            }
            this.lastID = -1;
            this.checkedList = new ArrayList();

            e.Cancel = false;
        }


        private void detailToolStripMenuItem_Click(object sender, EventArgs e )
        {
            string currentID_str = this.dataGridView1.CurrentCell.Value.ToString();
            int currentID = -1;

            if (Int32.TryParse(currentID_str, out currentID))
            {

                FilteredElementCollector elemTypeCtor = (new FilteredElementCollector(this.doc)).WhereElementIsElementType();
                FilteredElementCollector notElemTypeCtor = (new FilteredElementCollector(this.doc)).WhereElementIsNotElementType();
                FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith(notElemTypeCtor);
                ICollection<ElementId> ids = allElementCtor
                    .Where(el => el.Id.IntegerValue == currentID).Select(el => el.Id).ToList();
                if (ids.Count != 0)
                {
                    Snoop.Forms.Objects form = new Snoop.Forms.Objects(this.doc, ids);
                    form.ShowDialog();
                }
                else
                    MessageBox.Show($@"No items with ID {currentID} found");
            }
        }
    }
}
