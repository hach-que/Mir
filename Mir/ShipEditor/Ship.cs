namespace Mir
{
    using System.Collections.Generic;

    public class Ship
    {
        public Ship()
        {
            this.Cells = new List<ShipCell>();
        }

        public int MinimumX { get; set; }

        public int MaximumX { get; set; }

        public int MinimumY { get; set; }

        public int MaximumY { get; set; }

        public int MinimumZ { get; set; }
        
        public int MaximumZ { get; set; }

        public List<ShipCell> Cells { get; set; }
    }
}