namespace Mir
{
    using Protogame;

    public class RoomEditorSelectAction : IEventEntityAction<RoomEditorEntity>
    {
        public void Handle(IGameContext context, RoomEditorEntity entity, Event @event)
        {
            entity.SelectCurrentHover();
        }
    }
}