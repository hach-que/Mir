namespace Mir
{
    using Protogame;

    public class ShipToolReleaseAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (ShipEditorWorld)context.World;

            world.ReleaseTool(context);
        }
    }
}