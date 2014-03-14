namespace Mir
{
    public class ResizeSelectionShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Resize Selection";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.resize";
            }
        }
    }
}