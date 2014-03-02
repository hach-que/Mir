namespace Mir
{
    using System.Linq;
    using Protogame;

    public class JumpAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            if (world.ActiveTool is DCPUTool)
            {
                return;
            }

            var player = world.Entities.OfType<PlayerEntity>().First();

            player.InitiateJump();
        }
    }
}