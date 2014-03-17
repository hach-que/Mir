namespace Mir
{
    using System;

    public class NewRoomLEM1802Tool : NewRoomTool
    {
        public override string Name
        {
            get
            {
                return "New LEM1802";
            }
        }

        public override Type NewType
        {
            get
            {
                return typeof(LEM1802RoomObject);
            }
        }

        public override string TextureName
        {
            get
            {
                return "editor.room.tool.lem1802";
            }
        }
    }
}