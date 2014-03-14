namespace Mir
{
    using System;

    public class NewRoomLightTool : NewRoomTool
    {
        public override string Name
        {
            get
            {
                return "New Light";
            }
        }

        public override Type NewType
        {
            get
            {
                return typeof(LightRoomObject);
            }
        }
    }
}