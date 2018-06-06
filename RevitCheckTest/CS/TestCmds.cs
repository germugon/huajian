﻿#region Header
//
// Copyright 2003-2018 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
#endregion // Header

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Reflection;
using RevitLookup.Snoop.Forms;
using RevitLookup.AICheck.Forms;


// Each command is implemented as a class that provides the IExternalCommand Interface
//

namespace RevitLookup
{
  /// <summary>
  /// The classic bare-bones test.  Just brings up an Alert box to show that the connection to the external module is working.
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class HelloWorld : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Assembly a = Assembly.GetExecutingAssembly();
      string version = a.GetName().Version.ToString();
      TaskDialog helloDlg = new TaskDialog( "Autodesk Revit" );
      helloDlg.MainContent = "Hello World from " + a.Location + " v" + version;
      helloDlg.Show();
      return Result.Cancelled;
    }
  }

  /// <summary>
  /// SnoopDB command:  Browse all Elements in the current Document
  /// </summary>

  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class CmdSnoopDb : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Result result;

      try
      {
        Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;   // TBD: see note in CollectorExt.cs
        Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;

        // iterate over the collection and put them in an ArrayList so we can pass on
        // to our Form
        Autodesk.Revit.DB.Document doc = cmdData.Application.ActiveUIDocument.Document;
        FilteredElementCollector elemTypeCtor = ( new FilteredElementCollector( doc ) ).WhereElementIsElementType();
        FilteredElementCollector notElemTypeCtor = ( new FilteredElementCollector( doc ) ).WhereElementIsNotElementType();
        FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith( notElemTypeCtor );
        ICollection<Element> founds = allElementCtor.ToElements();

        ArrayList objs = new ArrayList();
        String Text = "";
        foreach( Element element in founds )
        {
          objs.Add( element );

        }
        System.Diagnostics.Trace.WriteLine( founds.Count.ToString() );
        Snoop.Forms.Objects form = new Snoop.Forms.Objects( objs );
        ActiveDoc.UIApp = cmdData.Application;
        form.ShowDialog();

        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }


  /// <summary>
  /// SnoopDB command:  Browse the current view...
  /// </summary>

  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class CmdSnoopActiveView : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Result result;

      try
      {
        Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;   // TBD: see note in CollectorExt.cs
        Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;

        // iterate over the collection and put them in an ArrayList so we can pass on
        // to our Form
        Autodesk.Revit.DB.Document doc = cmdData.Application.ActiveUIDocument.Document;
        if( doc.ActiveView == null )
        {
          TaskDialog.Show( "RevitLookup", "The document must have an active view!" );
          return Result.Cancelled;
        }

        Snoop.Forms.Objects form = new Snoop.Forms.Objects( doc.ActiveView );
        form.ShowDialog();

        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }

  /// <summary>
  /// Snoop ModScope command:  Browse all Elements in the current selection set
  ///                          In case nothing is selected: browse visible elements
  /// </summary>

  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class CmdSnoopModScope : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Result result;

      try
      {
        Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;
        UIDocument revitDoc = cmdData.Application.ActiveUIDocument;
        Document dbdoc = revitDoc.Document;
        Snoop.CollectorExts.CollectorExt.m_activeDoc = dbdoc; // TBD: see note in CollectorExt.cs
        Autodesk.Revit.DB.View view = dbdoc.ActiveView;

        //ElementSet ss = cmdData.Application.ActiveUIDocument.Selection.Elements; // 2015, jeremy: 'Selection.Selection.Elements' is obsolete: 'This property is deprecated in Revit 2015. Use GetElementIds() and SetElementIds instead.'
        //if (ss.Size == 0)
        //{
        //  FilteredElementCollector collector = new FilteredElementCollector( revitDoc.Document, view.Id );
        //  collector.WhereElementIsNotElementType();
        //  FilteredElementIterator i = collector.GetElementIterator();
        //  i.Reset();
        //  ElementSet ss1 = cmdData.Application.Application.Create.NewElementSet();
        //  while( i.MoveNext() )
        //  {
        //    Element e = i.Current as Element;
        //    ss1.Insert( e );
        //  }
        //  ss = ss1;
        //}

        ICollection<ElementId> ids = cmdData.Application.ActiveUIDocument.Selection.GetElementIds(); // 2016, jeremy
        if( 0 == ids.Count )
        {
          FilteredElementCollector collector
            = new FilteredElementCollector( revitDoc.Document, view.Id )
              .WhereElementIsNotElementType();
          ids = collector.ToElementIds();
        }

        //ICollection<Element> elements 
        //  = new List<Element>( ids.Select<ElementId,Element>( 
        //    id => dbdoc.GetElement( id ) ) );

        Snoop.Forms.Objects form = new Snoop.Forms.Objects( dbdoc, ids );
        ActiveDoc.UIApp = cmdData.Application;
        form.ShowDialog();

        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }

  /// <summary>
  /// Snoop App command:  Browse all objects that are part of the Application object
  /// </summary>

  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class CmdSnoopApp : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Result result;

      try
      {
        Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;
        Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;   // TBD: see note in CollectorExt.cs

        Snoop.Forms.Objects form = new Snoop.Forms.Objects( cmdData.Application.Application );
        form.ShowDialog();
        ActiveDoc.UIApp = cmdData.Application;
        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }

  /// <summary>
  /// Snoop ModScope command:  Browse all Elements in the current selection set
  /// </summary>

  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class CmdSampleMenuItemCallback : IExternalCommand
  {
    public Result Execute( ExternalCommandData cmdData, ref string msg, ElementSet elems )
    {
      Result result;

      try
      {
        MessageBox.Show( "Called back into RevitLookup by picking toolbar or menu item" );
        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }

  /// <summary>
  /// Search by and Snoop command: Browse 
  /// elements found by the condition
  /// </summary>
  [Transaction( TransactionMode.Manual )]
  [Regeneration( RegenerationOption.Manual )]
  public class CmdSearchBy : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData cmdData,
      ref string msg,
      ElementSet elems )
    {
      Result result;

      try
      {
        Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;
        UIDocument revitDoc = cmdData.Application.ActiveUIDocument;
        Document dbdoc = revitDoc.Document;
        Snoop.CollectorExts.CollectorExt.m_activeDoc = dbdoc; // TBD: see note in CollectorExt.cs

        SearchBy searchByWin = new SearchBy( dbdoc );
        ActiveDoc.UIApp = cmdData.Application;
        searchByWin.ShowDialog();
        result = Result.Succeeded;
      }
      catch( System.Exception e )
      {
        msg = e.Message;
        result = Result.Failed;
      }

      return result;
    }
  }



    /// <summary>
    /// Check command:  Browse all Elements in the current Document
    /// </summary>

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
  
    public class CmdCheck : IExternalCommand
    {
        Element GetTypeElement(Element element, Autodesk.Revit.DB.Document document)
        {
            Element elemType = document.GetElement(element.GetTypeId());
            return elemType;
        }
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Result result;

            try
            {
                Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;   // TBD: see note in CollectorExt.cs
                Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;

                // iterate over the collection and put them in an ArrayList so we can pass on
                // to our Form
                Autodesk.Revit.DB.Document doc = cmdData.Application.ActiveUIDocument.Document;
                FilteredElementCollector elemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsElementType();
                FilteredElementCollector notElemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsNotElementType();
                FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith(notElemTypeCtor);
                ICollection<Element> founds = allElementCtor.ToElements();
                Element tmpElement;
                String txt = "";
                ArrayList objs = new ArrayList();
                foreach (Element element in founds)
                {
                    
                    if (element.Category!=null && element.Category.Name!= null && Convert.ToString(element.Category.Name) == "门")
                    {
                        if (element.GetTypeId() == null)
                        {
                            continue;
                        }
                        Element typeelem = doc.GetElement(element.GetTypeId());
                        if (typeelem == null)
                        {
                            continue;
                        }
                        ParameterSet paramset = typeelem.Parameters;
                        if (paramset == null)
                        {
                            continue;
                        }
                        foreach (Parameter param in paramset)
                        {
                            if (param.Definition!=null&& param.Definition.Name!=null&&param.Definition.Name == "Width")
                            {
                                if (param.AsValueString() == null)
                                {
                                    continue;
                                }
                                String maynum = param.AsValueString();
                                int num = 0;
                                if (Int32.TryParse(maynum, out num))
                                {   
                                    if (num > 0)
                                    {
                                        txt = "break the rule \"The width of a residential building door should not be less than 0.90m\"";
                                        objs.Add(element);
                                        tmpElement = element;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                System.Diagnostics.Trace.WriteLine(founds.Count.ToString());
                Snoop.Forms.Objects form = new Snoop.Forms.Objects(objs);
                
                foreach (Element element in objs) {

                    int id = element.Id.IntegerValue;
                    ElementId el = new ElementId(id);
                    Transaction trans = new Transaction(doc);
                    trans.Start("ChangeColor");
                    Color color = new Color((byte)255, (byte)0, (byte)0);
                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    ogs.SetProjectionLineColor(color);
                    ogs.SetCutFillColor(color);
                    Autodesk.Revit.DB.View view = doc.ActiveView;
                    view.SetElementOverrides(el, ogs);

                    trans.Commit();

                }
                
                ActiveDoc.UIApp = cmdData.Application;
                form.ShowDialog();
                tmpElement = null;

                TaskDialog hintDlg = new TaskDialog("Check Hint");
                hintDlg.MainContent = txt;
                hintDlg.Show();

                result = Result.Succeeded;
            }
            catch (System.Exception e)
            {
                msg = e.Message;
                result = Result.Failed;
                return result;
            }
            return result;
        }
    }


    /// <summary>
    /// CheckTest command:  Browse all Elements in the current Document
    /// </summary>

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CmdCheckerByZW : IExternalCommand
    {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Result result;

            try
            {
                Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;   // TBD: see note in CollectorExt.cs
                Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;

                // iterate over the collection and put them in an ArrayList so we can pass on
                // to our Form
                Autodesk.Revit.DB.Document doc = cmdData.Application.ActiveUIDocument.Document;
                FilteredElementCollector elemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsElementType();
                FilteredElementCollector notElemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsNotElementType();
                FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith(notElemTypeCtor);
                ICollection<Element> founds = allElementCtor.ToElements();
                Element tmpElement;
                ArrayList objs = new ArrayList();


                foreach (Element element in founds)
                {

                    if (element.Category != null && element.Category.Name != null && Convert.ToString(element.Category.Name).Contains("门"))
                    {
                        if (element.GetTypeId() == null)
                        {
                            continue;
                        }
                        Element typeelem = doc.GetElement(element.GetTypeId());
                        if (typeelem == null)
                        {
                            continue;
                        }
                        ParameterSet paramset = typeelem.Parameters;
                        if (paramset == null)
                        {
                            continue;
                        }
                        foreach (Parameter param in paramset)
                        {
                            if (param.Definition != null && param.Definition.Name != null && param.Definition.Name.Contains("宽度"))
                            {
                                if (param.AsValueString() == null)
                                {
                                    continue;
                                }
                                String maynum = param.AsValueString();
                                int num = 0;
                                if (Int32.TryParse(maynum, out num))
                                {
                                    if (num > 0)
                                    {

                                        objs.Add(element);
                                        tmpElement = element;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                System.Diagnostics.Trace.WriteLine(founds.Count.ToString());
                Snoop.Forms.Check form = new Snoop.Forms.Check(objs,cmdData, doc);



                ActiveDoc.UIApp = cmdData.Application;
                form.ShowDialog();
                tmpElement = null;
               

                result = Result.Succeeded;
            }
            catch (System.Exception e)
            {
                msg = e.Message;
                result = Result.Failed;
                return result;
            }
            return result;
        }
    }


    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdSelectRVTPath : IExternalCommand
    {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Result result;

            try
            {
                //Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;
                //UIDocument revitDoc = cmdData.Application.ActiveUIDocument;
                //Document dbdoc = revitDoc.Document;
                //Snoop.CollectorExts.CollectorExt.m_activeDoc = dbdoc; // TBD: see note in CollectorExt.cs
                //ActiveDoc.UIApp = cmdData.Application;

                CheckRvt selectFile = new CheckRvt(cmdData);
                selectFile.ShowDialog();
                result = Result.Succeeded;
            }
            catch (System.Exception e)
            {
                msg = e.Message;
                result = Result.Failed;
            }

            return result;
        }
    }


}
