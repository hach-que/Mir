namespace Mir
{
    using System;

    public class NewRoomDCPUTool : NewRoomTool
    {
        public override string Name
        {
            get
            {
                return "New DCPU";
            }
        }

        public override Type NewType
        {
            get
            {
                return typeof(DCPURoomObject);
            }
        }

        public override string TextureName
        {
            get
            {
                return "editor.room.tool.dcpu";
            }
        }
    }
}