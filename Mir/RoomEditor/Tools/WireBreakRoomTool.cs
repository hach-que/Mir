namespace Mir
{
    public class WireBreakRoomTool : IRoomTool
    {
        public string Name
        {
            get
            {
                return "Break Wiring";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.room.tool.wirebreak";
            }
        }
    }
}