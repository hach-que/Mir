namespace Mir
{
    public class ExitRoomTool : IRoomTool
    {
        public string Name
        {
            get
            {
                return "Exit to Ship";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.room.tool.exit";
            }
        }
    }
}