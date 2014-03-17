namespace Mir
{
    using System;

    public class NewRoomKeyboardTool : NewRoomTool
    {
        public override string Name
        {
            get
            {
                return "New Keyboard";
            }
        }

        public override Type NewType
        {
            get
            {
                return typeof(KeyboardRoomObject);
            }
        }

        public override string TextureName
        {
            get
            {
                return "editor.room.tool.key";
            }
        }
    }
}