namespace Mir
{
    using System;

    public class NewRoomSPED3Tool : NewRoomTool
    {
        public override string Name
        {
            get
            {
                return "New SPED-3";
            }
        }

        public override Type NewType
        {
            get
            {
                return typeof(SPED3RoomObject);
            }
        }

        public override string TextureName
        {
            get
            {
                return "editor.room.tool.sped3";
            }
        }
    }
}