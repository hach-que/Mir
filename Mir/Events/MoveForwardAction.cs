using System;
using Protogame;
using System.Linq;

namespace Mir
{
    public class MoveForwardAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().First();

            player.X += player.ForwardVector.X * player.MovementSpeed;
            player.Z += player.ForwardVector.Z * player.MovementSpeed;
            player.Walked = true;
            player.Constrain();
        }
    }
}

