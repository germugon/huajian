using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace RevitLookup.AICheck.Utils
{
	class CustomLevel
	{
		//首层
		public static Boolean isFirstFloor(Document _doc, Element element)
		{
			//Door has LevelId????
			Level level = _doc.GetElement(element.LevelId) as Level;
			if (level != null && level.Name != null && level.Name.Contains("1F"))
				return true;
			else
				return false;
		}

		//标高
		public static ICollection<Element> getLevels(Document _doc)
		{
			FilteredElementCollector collector = new FilteredElementCollector(_doc);
			return collector.OfClass(typeof(Level)).ToElements();
		}
	}
}
