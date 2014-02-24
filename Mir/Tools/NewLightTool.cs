namespace Mir
{
    using System;

    public class NewLightTool : NewTool
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