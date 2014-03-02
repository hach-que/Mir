namespace Mir
{
    using Protogame;

    public class MainMenuStaticEventBinder : StaticEventBinder<IGameContext>
    {
        public override void Configure()
        {
            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Left).To<MenuSelectAction>();
        }

        protected override bool Filter(IGameContext context, Event @event)
        {
            return context.World is MainMenuWorld;
        }
    }
}