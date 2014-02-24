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

        public string TextureName
        {
            get
            {
                return "new";
            }
        }

        public virtual Type NewType
        {
            get
            {
                return typeof(RoomObject);
            }
        }
    }
}