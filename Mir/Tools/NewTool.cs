namespace Mir
{
    using System;

    public class NewTool : ITool
    {
        public virtual string Name
        {
            get
            {
                return "New Object";
            }
        }

        public virtual Type NewType
        {
            get
            {
                return typeof(RoomObject);
            }
        }

        public string TextureName
        {
            get
            {
                return "new";
            }
        }
    }
}