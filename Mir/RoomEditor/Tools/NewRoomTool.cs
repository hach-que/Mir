namespace Mir
{
    using System;

    public class NewRoomTool : IRoomTool
    {
        public virtual string Name
        {
            get
            {
                return "New Object";
            }
        }

        public virtual Type NewType
        {
            get
            {
                return typeof(RoomObject);
            }
        }

        public virtual string TextureName
        {
            get
            {
                return "editor.room.tool.new";
            }
        }
    }
}