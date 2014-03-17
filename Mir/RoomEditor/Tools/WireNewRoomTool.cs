namespace Mir
{
    public class WireNewRoomTool : IRoomTool
    {
        public string Name
        {
            get
            {
                return "New Wiring";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.room.tool.wirenew";
            }
        }
    }
}