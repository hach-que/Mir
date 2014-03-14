namespace Mir
{
    using System.Collections.Generic;

    public class Ship
    {
        public Ship()
        {
            this.Rooms = new List<Room>();
        }

        public List<Room> Rooms { get; private set; }
    }
}