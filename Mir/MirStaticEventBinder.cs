namespace Mir
{
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class MirStaticEventBinder : StaticEventBinder<IGameContext>
    {
        public override void Configure()
        {
            this.Bind<MouseMoveEvent>(x => true).To<MouseMoveAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.W).To<MoveForwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.S).To<MoveBackwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.A).To<StrafeLeftAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.D).To<StrafeRightAction>();
            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Right).To<ToggleMouseAction>();

            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Left)
                .On<RoomEditorEntity>()
                .To<RoomEditorSelectAction>();

            this.Bind<MouseReleaseEvent>(x => x.Button == MouseButton.Left)
                .On<RoomEditorEntity>()
                .To<RoomEditorReleaseAction>();
        }
    }
}