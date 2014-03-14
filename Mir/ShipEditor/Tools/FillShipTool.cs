namespace Mir
{
    public class FillShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Fill Space";
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