using System;
using Protogame;

namespace Mir
{
    public class TestPhysicsAction : IEventAction<IGameContext>
    {
        public void Handle(IGameContext context, Event @event)
        {
            var world = (RoomEditorWorld)context.World;

            world.TestPhysics();
        }
    }
}

