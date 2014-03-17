namespace Mir
{
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    /// <summary>
    /// The mir static event binder.
    /// </summary>
    public class RoomEditorStaticEventBinder : StaticEventBinder<IGameContext>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        public override void Configure()
        {
            this.Bind<MouseMoveEvent>(x => true).To<MouseMoveRoomAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.W).To<MoveForwardRoomAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.S).To<MoveBackwardRoomAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.A).To<StrafeLeftRoomAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.D).To<StrafeRightRoomAction>();

            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Left || x.Button == MouseButton.Right)
                .To<RoomToolUseAction>();

            this.Bind<MouseReleaseEvent>(x => x.Button == MouseButton.Left || x.Button == MouseButton.Right)
                .To<RoomToolReleaseAction>();

            this.Bind<KeyPressEvent>(x => x.Key == Keys.LeftShift).To<RoomToolAlternateAction>();
            this.Bind<KeyReleaseEvent>(x => x.Key == Keys.LeftShift).To<RoomToolAlternateAction>();

            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Space).To<JumpRoomAction>();
        }

        protected override bool Filter(IGameContext context, Event @event)
        {
            var roomEditorWorld = context.World as RoomEditorWorld;

            if (roomEditorWorld == null)
            {
                return false;
            }

            if (@event is KeyboardEvent && roomEditorWorld.FocusedKeyboard != null)
            {
                if (!(roomEditorWorld.ActiveRoomTool is MoveRoomTool))
                {
                    return false;
                }
            }

            return true;
        }
    }
}