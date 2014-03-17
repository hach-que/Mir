namespace Mir
{
    using System.Linq;
    using Protogame;

    public class StrafeRightRoomAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            var player = world.Entities.OfType<PlayerEntity>().First();

            player.Walked = true;
            player.ApplyDirection(player.RightVector * player.MovementSpeed);
        }
    }
}