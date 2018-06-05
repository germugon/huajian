using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitLookup.Snoop.Forms
{
    public partial class CheckTestForm : System.Windows.Forms.Form
    {
        public ExternalCommandData cmdData = null;
        public CheckTestForm(ArrayList objs, ExternalCommandData cmdData)
        {
            this.cmdData = cmdData;
            InitializeComponent();
            CommonInit(objs);
        }

        private void CommonInit(ArrayList objs)
        {
            this.dataGridView1.DataSource = createSource(objs);
            
            this.dataGridView1.Columns[0].FillWeight = 15;
            this.dataGridView1.Columns[1].FillWeight = 85;
            this.dataGridView1.Update();
        }

        private DataTable createSource(ArrayList obj)
        {
            
            DataTable dt = new DataTable();  
            dt.Columns.Add("ID", System.Type.GetType("System.Int32"));
            DataColumn dc = new DataColumn("Rule", System.Type.GetType("System.String"));
            dt.Columns.Add(dc); 
            

            foreach (Element element in obj)
            {
                DataRow dr = dt.NewRow();
                dr[0] = element.Id.IntegerValue; ;
                dr[1] = "Break Rule(s):\n  1)The width of a residential building door should not be less than 0.90m";
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private int lastID = -1;

        private ArrayList IDList = new ArrayList();
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string i = this.dataGridView1.CurrentCell.Value.ToString();
            int j;
            if (Int32.TryParse(i, out j))
            {
                UIApplication app = this.cmdData.Application;
                Document doc = app.ActiveUIDocument.Document;

                ElementId el = new ElementId(j);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor1");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)255, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetProjectionFillColor(color);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(el, ogs);
                trans.Commit();
                this.IDList.Add(j);
            }
            if (this.lastID != -1)
            {
                UIApplication app = this.cmdData.Application;
                Document doc = app.ActiveUIDocument.Document;

                ElementId el = new ElementId(this.lastID);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor2");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)0, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetProjectionFillColor(color);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(el, ogs);
                trans.Commit();
            }
            if (Int32.TryParse(i, out j))
            {
                this.lastID = j;
            }
            File.WriteAllText("c:\\1.txt",i);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (int id in this.IDList)
            {
                UIApplication app = this.cmdData.Application;
                Document doc = app.ActiveUIDocument.Document;

                ElementId el = new ElementId(id);

                Transaction trans = new Transaction(doc);
                trans.Start("ChangeColor1");
                Autodesk.Revit.DB.Color color = new Autodesk.Revit.DB.Color((byte)0, (byte)0, (byte)0);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(color);
                ogs.SetCutFillColor(color);
                ogs.SetProjectionFillColor(color);
                Autodesk.Revit.DB.View view = doc.ActiveView;
                view.SetElementOverrides(el, ogs);
                trans.Commit();
            }
            this.lastID = -1;
            this.IDList = new ArrayList();

            this.Close();
        }
    }
}
