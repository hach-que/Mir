namespace Mir
{
    public class PencilClearShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Clear Space (Pencil)";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.emptypen";
            }
        }
    }
}