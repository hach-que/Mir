namespace Mir
{
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    /// <summary>
    /// The mir static event binder.
    /// </summary>
    public class ShipEditorStaticEventBinder : StaticEventBinder<IGameContext>
    {
        /// <summary>
        /// The configure.
        /// </summary>
        public override void Configure()
        {
            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Left || x.Button == MouseButton.Right)
                .To<ShipToolUseAction>();

            this.Bind<MouseReleaseEvent>(x => x.Button == MouseButton.Left || x.Button == MouseButton.Right)
                .To<ShipToolReleaseAction>();

            this.Bind<KeyPressEvent>(x => x.Key == Keys.LeftShift).To<ShipToolAlternateAction>();
            this.Bind<KeyReleaseEvent>(x => x.Key == Keys.LeftShift).To<ShipToolAlternateAction>();
        }

        protected override bool Filter(IGameContext context, Event @event)
        {
            return context.World is ShipEditorWorld;
        }
    }
}