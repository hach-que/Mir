namespace Mir
{
    using Protogame;

    public class ShipToolUseAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (ShipEditorWorld)context.World;

            var mouseEvent = @event as MousePressEvent;

            var alt = mouseEvent != null && mouseEvent.Button == MouseButton.Right;

            world.UseTool(context, alt);
        }
    }
}