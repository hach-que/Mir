namespace Mir
{
    public class PencilFillShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Fill Space (Pencil)";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.fillpen";
            }
        }
    }
}