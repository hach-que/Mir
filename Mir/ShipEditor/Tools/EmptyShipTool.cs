namespace Mir
{
    public class EmptyShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Clear Space";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.empty";
            }
        }
    }
}