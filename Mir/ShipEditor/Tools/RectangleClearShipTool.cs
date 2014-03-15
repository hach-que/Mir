namespace Mir
{
    public class RectangleClearShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Clear Space (Rectangle)";
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