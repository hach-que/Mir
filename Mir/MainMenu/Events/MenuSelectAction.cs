namespace Mir
{
    using Protogame;

    public class MenuSelectAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var mainMenuWorld = (MainMenuWorld)context.World;

            mainMenuWorld.Select(context);
        }
    }
}