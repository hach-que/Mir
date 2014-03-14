namespace Mir
{
    using Protogame;

    public class ShipToolAlternateAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (ShipEditorWorld)context.World;

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