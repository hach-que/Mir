using System;
using System.Collections.Generic;

namespace Mir
{
    /// <summary>
    /// http://www.0x10cportal.com/460-0x10c-room-editor-demo
    /// 
    /// A room is made of 6 walls facing inward and a series of objects that are
    /// used to shape the walls.  The room can be of any size, but adheres to a
    /// grid both for it's shape and for the shape of objects internally (such that
    /// it is easy for players to modify objects).
    ///  
    /// Each object is a cube, and players can interact with them to change the
    /// dimension of any axis, move the cube, or flag corners as omitted to
    /// cause the object to form diagonal shapes.
    /// </summary>
    public class Room
    {
        private readonly IEnumerable<RoomObject> m_RoomObjects;

        public Room()
        {
        }
    }
}

