namespace Mir
{
    public class ExitShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Exit to Ship Select";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.exit";
            }
        }
    }
}