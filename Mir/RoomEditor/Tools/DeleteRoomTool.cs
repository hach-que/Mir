namespace Mir
{
    public class DeleteRoomTool : IRoomTool
    {
        public string Name
        {
            get
            {
                return "Delete Object";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.room.tool.delete";
            }
        }
    }
}