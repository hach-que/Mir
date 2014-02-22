namespace Mir
{
    using Protogame;

    public class ToolUseAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            world.UseTool();
        }
    }
}