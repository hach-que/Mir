namespace Mir
{
    public class RotateViewShipTool : IShipTool
    {
        public string Name
        {
            get
            {
                return "Rotate View";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.ship.tool.rotate";
            }
        }
    }
}