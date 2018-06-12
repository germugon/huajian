using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitLookup.AICheck.Utils;

namespace RevitLookup.AICheck.Entity
{
	class Corridor
	{
		public static List<Room> getAllCorridor(Document _doc)
		{
			RoomFilter filter = new RoomFilter();
			FilteredElementCollector collector = new FilteredElementCollector(_doc);
			IList<Element> elements = collector.WherePasses(filter).ToElements();

			List<Room> corridors = new List<Room>();
			foreach (Element elem in elements)
			{
				Room room = elem as Room;
				if (isCorridor(room))
					corridors.Add(room);
			}
			return corridors;
		}

		//////////////////////////////////////////Revise////////////////////////////////////////////////////
		public static Boolean isCorridor(Room room)
		{
			if (room != null && room.Name != null && room.Name.Contains("走道"))
				return true;
			else
				return false;
		}

		public static Boolean isEvacuationCorridor(Room room)
		{
			//Evacuation????
			return isCorridor(room);
		}

		public static Double getCorridorWidth(Stairs stairs)
		{
			//to do....
			return 0;
		}
		//////////////////////////////////////////End////////////////////////////////////////////////////
	}
}
