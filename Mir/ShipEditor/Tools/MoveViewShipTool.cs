namespace Mir
{
    public class MoveViewShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Move View";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.move";
            }
        }
    }
}