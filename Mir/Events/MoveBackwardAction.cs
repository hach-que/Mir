using System;
using Protogame;
using System.Linq;

namespace Mir
{
    public class MoveBackwardAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().First();

            if (!player.CaptureMouse)
            {
                return;
            }

            player.X -= player.ForwardVector.X * player.MovementSpeed;
            player.Z -= player.ForwardVector.Z * player.MovementSpeed;
            player.Walked = true;
            player.Constrain();
        }
    }
}

