namespace Mir
{
    public class DeleteRoomShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Delete Room";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.roomdel";
            }
        }
    }
}