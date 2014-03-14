namespace Mir
{
    using System.Linq;
    using Protogame;

    public class MoveBackwardRoomAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            if (world.ActiveRoomTool is DCPURoomTool)
            {
                return;
            }

            var player = world.Entities.OfType<PlayerEntity>().First();

            player.Walked = true;
            player.ApplyDirection(-player.ForwardVector * player.MovementSpeed);
        }
    }
}