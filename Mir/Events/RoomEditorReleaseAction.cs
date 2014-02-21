namespace Mir
{
    using Protogame;

    public class RoomEditorReleaseAction : IEventEntityAction<RoomEditorEntity>
    {
        public void Handle(IGameContext context, RoomEditorEntity entity, Event @event)
        {
            entity.ReleaseCurrentSelection();
        }
    }
}