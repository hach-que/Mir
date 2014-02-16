using System;
using Protogame;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class MouseMoveAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var mouseEvent = @event as MouseMoveEvent;

            var viewport = ((MirGame)context.Game).RenderContext.GraphicsDevice.Viewport;

            var diffX = mouseEvent.X - viewport.Width / 2;
            var diffY = mouseEvent.Y - viewport.Height / 2;

            var player = context.World.Entities.OfType<PlayerEntity>().First();

            if (!player.CaptureMouse)
            {
                return;
            }

            player.TargetYaw -= diffX / ((float)viewport.Width * 0.75f);
            player.TargetPitch -= diffY / ((float)viewport.Height * 0.75f);

            player.TargetPitch = MathHelper.Clamp(player.TargetPitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);

            Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);
        }
    }
}

