namespace Mir
{
    public class CreateRoomShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Create Room";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.room";
            }
        }
    }
}