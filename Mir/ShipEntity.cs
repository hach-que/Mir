using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class ShipEntity : IEntity, IArea
    {
        private readonly ShipLayout m_ShipLayout;

        public ShipEntity(IFactory factory)
        {
            this.m_ShipLayout = factory.CreateShipLayout(10);
            this.m_ShipLayout.FillArea(3, 0, 3, 3, 1, 3);
            this.m_ShipLayout.FillArea(0, 0, 4, 3, 1, 1);
            //this.m_ShipLayout.RunCorridor(0, 0, 4, LayoutDirection.Right, 6);
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
            this.m_ShipLayout.Render(
                renderContext,
                Matrix.CreateScale(10));
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }
    }
}

