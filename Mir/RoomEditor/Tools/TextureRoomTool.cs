namespace Mir
{
    public class TextureRoomTool : IRoomTool
    {
        public string Name
        {
            get
            {
                return "Change Texture";
            }
        }

        public string TextureName
        {
            get
            {
                return "editor.room.tool.texture";
            }
        }
    }
}