using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class ShipEntity : IEntity, IArea
    {
        private readonly Room m_Room;

        private readonly IMeshCollider m_MeshCollider;

        private readonly I3DRenderUtilities m_3DRenderUtilities;

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
            if (!renderContext.Is3DContext)
            {
                return;
            }

            this.m_Room.Render(renderContext);

            Vector3 position;
            if (this.m_MeshCollider.Collides(gameContext.MouseRay, this.m_Room, out position))
            {
                this.m_3DRenderUtilities.RenderCube(
                    renderContext,
                    Matrix.CreateTranslation(position),
                    Color.White);
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}

