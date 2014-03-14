namespace Mir
{
    using Protogame;

    public class RoomToolReleaseAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            world.ReleaseTool();
        }
    }
}