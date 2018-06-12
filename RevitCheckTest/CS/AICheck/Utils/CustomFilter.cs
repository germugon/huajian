using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitLookup.AICheck.Utils
{
	class CustomFilter
	{
		public static IEnumerable<T> getElements<T>(Document _doc, ElementFilter filter, BuiltInCategory builtInCategory) where T : Element
		{
			FilteredElementCollector collector = new FilteredElementCollector(_doc);
			// Seems you must be a subclass of element to use the OfClass method
			if (typeof(T) != typeof(Element))
				collector.OfClass(typeof(T));
			collector.WherePasses(filter).OfCategory(builtInCategory);
			return collector.Cast<T>();
		}
	}
}
