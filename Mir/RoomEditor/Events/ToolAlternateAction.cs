namespace Mir
{
    using Protogame;

    public class ToolAlternateAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            if (@event is KeyPressEvent)
            {
                world.ActivateAlternate();
            }
            else
            {
                world.DeactivateAlternate();
            }
        }
    }
}