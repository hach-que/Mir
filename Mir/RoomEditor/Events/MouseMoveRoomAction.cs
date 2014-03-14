namespace Mir
{
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class MouseMoveRoomAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var mouseEvent = @event as MouseMoveEvent;

            if (mouseEvent == null)
            {
                return;
            }

            if (!(context.World is RoomEditorWorld))
            {
                return;
            }

            var viewport = ((MirGame)context.Game).RenderContext.GraphicsDevice.Viewport;

            var diffX = mouseEvent.X - (viewport.Width / 2);
            var diffY = mouseEvent.Y - (viewport.Height / 2);

            var player = context.World.Entities.OfType<PlayerEntity>().First();

            player.TargetYaw -= diffX / (viewport.Width * 0.75f);
            player.TargetPitch -= diffY / (viewport.Height * 0.75f);

            player.TargetPitch = MathHelper.Clamp(
                player.TargetPitch, 
                -MathHelper.PiOver2 + 0.01f, 
                MathHelper.PiOver2 - 0.01f);

            Mouse.SetPosition(viewport.Width / 2, viewport.Height / 2);
        }
    }
}