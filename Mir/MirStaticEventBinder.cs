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

            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Left || x.Button == MouseButton.Right)
                .To<ToolUseAction>();

            this.Bind<MouseReleaseEvent>(x => x.Button == MouseButton.Left)
                .To<ToolReleaseAction>();

            this.Bind<KeyPressEvent>(x => x.Key == Keys.LeftShift).To<ToolAlternateAction>();
            this.Bind<KeyReleaseEvent>(x => x.Key == Keys.LeftShift).To<ToolAlternateAction>();

            this.Bind<KeyHeldEvent>(x => x.Key == Keys.E).To<TestPhysicsAction>();

            this.Bind<KeyHeldEvent>(x => x.Key == Keys.Space).To<JumpAction>();
        }
    }
}