namespace Mir
{
    using System.Linq;
    using Protogame;

    public class StrafeRightAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            if (world.ActiveTool is DCPUTool)
            {
                return;
            }

            var player = world.Entities.OfType<PlayerEntity>().First();

            player.X += player.RightVector.X * player.MovementSpeed;
            player.Z += player.RightVector.Z * player.MovementSpeed;
            player.Walked = true;
            player.Constrain();
        }
    }
}