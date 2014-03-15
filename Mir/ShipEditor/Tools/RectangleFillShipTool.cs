namespace Mir
{
    public class RectangleFillShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Fill Space (Rectangle)";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.fill";
            }
        }
    }
}