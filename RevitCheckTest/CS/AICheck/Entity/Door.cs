using System;
using Autodesk.Revit.DB;

namespace RevitLookup.AICheck.Entity
{
	class Door
	{
		public static Boolean getEgressDoor(Element fromRoom, Element toRoom)
		{
			if (getExteriorDoor(fromRoom, toRoom) || getStairDoor(fromRoom, toRoom) || getCorridorDoor(fromRoom, toRoom))
				return true;
			else
				return false;
		}

		public static Boolean getExteriorDoor(Element fromRoom, Element toRoom)
		{
			if ((fromRoom != null && toRoom == null) || (fromRoom == null && toRoom != null))
				return true;
			else
				return false;
		}

		public static Boolean getStairDoor(Element fromRoom, Element toRoom)
		{
			if ((fromRoom != null && fromRoom.Name != null && fromRoom.Name.Contains("楼梯间")) || (toRoom != null && toRoom.Name != null && toRoom.Name.Contains("楼梯间")))
				return true;
			else
				return false;
		}

		public static Boolean getCorridorDoor(Element fromRoom, Element toRoom)
		{
			if ((fromRoom != null && fromRoom.Name != null && fromRoom.Name.Contains("走道")) || (toRoom != null && toRoom.Name != null && toRoom.Name.Contains("走道")))
				return true;
			else
				return false;
		}
	}
}
