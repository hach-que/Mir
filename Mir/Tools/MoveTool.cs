namespace Mir
{
    using Protogame;

    public class MoveTool : ITool
    {
        public string Name
        {
            get
            {
                return "Move Object";
            }
        }

        public string TextureName
        {
            get
            {
                return "move";
            }
        }
    }
}