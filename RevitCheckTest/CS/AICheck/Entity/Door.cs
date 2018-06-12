using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitLookup.AICheck.Utils;

namespace RevitLookup.AICheck.Entity
{
	class Door
	{
		public static string[] excludeRoomName = {"弱电", "强电"};

		public static IEnumerable<Element> getAllDoor(Document _doc)
		{
			FilteredElementCollector collector = new FilteredElementCollector(_doc);
			ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
			FilteredElementCollector elements = collector.WherePasses(filter).OfCategory(BuiltInCategory.OST_Doors);
			foreach (Element elem in elements)
			{
				//Cast to FamilyInstance is safe because ElementClassFilter for FamilyInstance was used
				FamilyInstance familyInstance = elem as FamilyInstance;

			}
		}

		//Considering Irregular Door(fromRoom == null && toRoom == null)
		public static Boolean isTagFromToDoor(FamilyInstance familyInstance)
		{
			Element fromRoom = familyInstance.FromRoom;
			Element toRoom = familyInstance.ToRoom;
			if (fromRoom == null && toRoom == null)
				return false;
			else
				return true;
		}

		//////////////////////////////////////////Revise////////////////////////////////////////////////////
		public static Boolean isEvacuationDoor(Document _doc, Element element)
		{
			//Cast to FamilyInstance is safe because ElementClassFilter for FamilyInstance was used
			FamilyInstance familyInstance = element as FamilyInstance;
			if (isExteriorDoor(_doc, familyInstance) || isEvacuationStairDoor(familyInstance) || isRoomToCorridorDoor(familyInstance))
				return true;
			else
				return false;
		}

		public static Boolean isExit(Document _doc, Element element)
		{
			//Cast to FamilyInstance is safe because ElementClassFilter for FamilyInstance was used
			FamilyInstance familyInstance = element as FamilyInstance;
			if (isEvacuationStairDoor(familyInstance) || isEvacuationCorridorDoor(familyInstance) || isFrontRoomDoor(familyInstance) || isExteriorDoor(_doc, familyInstance))
				return true;
			else
				return false;
		}

		public static Boolean isExteriorDoor(Document _doc, FamilyInstance familyInstance)
		{
			//first floor????
			//exlude room named "弱电"????
			Element fromRoom = familyInstance.FromRoom;
			Element toRoom = familyInstance.ToRoom;
			if (CustomLevel.isFirstFloor(_doc, familyInstance) && ((fromRoom != null && toRoom == null) || (fromRoom == null && toRoom != null)))
				return true;
			else
				return false;
		}

		public static Boolean isEvacuationStairDoor(FamilyInstance familyInstance)
		{
			//"楼梯间" not exist!!!!
			if (checkDoorType(familyInstance, "楼梯"))
				return true;
			else
				return false;
		}

		public static Boolean isRoomToCorridorDoor(FamilyInstance familyInstance)
		{
			//which room means "房间"????
			//exlude room named "弱电"????
			if (checkDoorType(familyInstance, "房间", "走道") || checkDoorType(familyInstance, "走道", "房间"))
				return true;
			else
				return false;
		}

		public static Boolean isEvacuationCorridorDoor(FamilyInstance familyInstance)
		{
			if (checkDoorType(familyInstance, "走道"))
				return true;
			else
				return false;
		}
		
		public static Boolean isFrontRoomDoor(FamilyInstance familyInstance)
		{
			if (checkDoorType(familyInstance, "前室"))
				return true;
			else
				return false;
		}
		//////////////////////////////////////////Revise////////////////////////////////////////////////////

		public static Boolean checkDoorType(FamilyInstance familyInstance, string roomName)
		{
			Element fromRoom = familyInstance.FromRoom;
			Element toRoom = familyInstance.ToRoom;
			if ((fromRoom != null && fromRoom.Name != null && fromRoom.Name.Contains(roomName))
				|| (toRoom != null && toRoom.Name != null && toRoom.Name.Contains(roomName)))
				return true;
			else
				return false;
		}

		public static Boolean checkDoorType(FamilyInstance familyInstance, string fromRoomName, string toRoomName)
		{
			Element fromRoom = familyInstance.FromRoom;
			Element toRoom = familyInstance.ToRoom;
			if ((fromRoom != null && fromRoom.Name != null && fromRoom.Name.Contains(fromRoomName)) 
				&& (toRoom != null && toRoom.Name != null && toRoom.Name.Contains(toRoomName)))
				return true;
			else
				return false;
		}
	}
}
