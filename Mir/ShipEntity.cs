using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class ShipEntity : IEntity, IArea
    {
        public ShipEntity(IFactory factory)
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

