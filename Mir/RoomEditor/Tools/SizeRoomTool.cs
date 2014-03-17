namespace Mir
{
    public class SizeRoomTool : IRoomTool
    {
        public string Name
        {
            get
            {
                return "Adjust Size";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.room.tool.resize";
            }
        }
    }
}