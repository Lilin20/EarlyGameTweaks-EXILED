using System;
using System.Collections.Generic;
using System.Linq; // Added for LINQ
using Exiled.API.Enums;
using Exiled.API.Features;

namespace EarlyGameTweaks
{
    public class RoomManager
    {
        public static List<Room> RoomsInZone(ZoneType zone)
        {
            return Room.List.Where(room => room.Zone == zone).ToList();
        }

        public static List<Room> GetRandomRoomsInZone(ZoneType zone, int count)
        {
            List<Room> roomsInZone = RoomsInZone(zone);

            if (roomsInZone.Count == 0 || count <= 0)
            {
                return new List<Room>();
            }

            return roomsInZone.OrderBy(_ => Guid.NewGuid()).Take(count).ToList();
        }
    }
}
