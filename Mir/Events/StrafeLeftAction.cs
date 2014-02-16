using System;
using Protogame;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class StrafeLeftAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().First();

            player.X += player.LeftVector.X * player.MovementSpeed;
            player.Z += player.LeftVector.Z * player.MovementSpeed;
            player.Walked = true;
            player.Constrain();
        }
    }
}

