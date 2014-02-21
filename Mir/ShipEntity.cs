using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Mir
{
    using System.Linq;

    public class ShipEntity : IEntity, IArea
    {
        private readonly Room m_Room;

        private readonly IMeshCollider m_MeshCollider;

        private readonly I3DRenderUtilities m_3DRenderUtilities;

        private RoomObject m_LastRoomObject;

        public ShipEntity(
            I3DRenderUtilities threedRenderUtilities,
            IMeshCollider meshCollider,
            IFactory factory)
        {
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_MeshCollider = meshCollider;
            this.m_Room = factory.CreateRoom();
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}

