using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitLookup.AICheck.Utils;

namespace RevitLookup.AICheck.Entity
{
	class Stair
	{
		public static List<Stairs> getAllStair(Document _doc)
		{
			FilteredElementCollector collector = new FilteredElementCollector(_doc);
			ICollection<ElementId> stairsIds = collector.WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Stairs).ToElementIds();
			List<Stairs> stairs = new List<Stairs>();
			foreach (ElementId stairId in stairsIds)
			{
				if (Stairs.IsByComponent(_doc, stairId) == true)
				{
					Stairs stair = _doc.GetElement(stairId) as Stairs;
					stairs.Add(stair);
				}
			}
			return stairs;
		}

		//////////////////////////////////////////Revise////////////////////////////////////////////////////
		public static Boolean isEvacuationStair(Stairs stairs)
		{
			//Evacuation????
			if (stairs != null)
				return true;
			else
				return false;
		}

		public static Double getStairWidth(Stairs stairs)
		{
			//MinRunWidth????
			StairsType stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
			return stairsType.MinRunWidth;
		}
		//////////////////////////////////////////End////////////////////////////////////////////////////
	}
}
