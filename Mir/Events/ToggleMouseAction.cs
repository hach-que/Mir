using System;
using Protogame;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class ToggleMouseAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var player = context.World.Entities.OfType<PlayerEntity>().First();

            player.CaptureMouse = !player.CaptureMouse;
        }
    }
}

