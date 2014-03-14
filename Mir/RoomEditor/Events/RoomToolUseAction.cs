namespace Mir
{
    using Protogame;

    public class RoomToolUseAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            var mouseEvent = @event as MousePressEvent;

            var alt = mouseEvent != null && mouseEvent.Button == MouseButton.Right;

            world.UseTool(context, alt);
        }
    }
}