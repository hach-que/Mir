using System;
using Protogame;

namespace Mir
{
    public class ShipEntity : IEntity, IArea
    {
        private readonly ShipLayout m_ShipLayout;

        public ShipEntity()
        {
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

