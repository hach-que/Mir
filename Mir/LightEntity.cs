namespace Mir
{
    using System;
    using Microsoft.Xna.Framework;
    using Protogame;

    public class LightEntity : IEntity, ILight
    {
        private readonly LightRoomObject m_LightRoomObject;

        private IGameContext m_LastGameContext;

        public LightEntity(LightRoomObject lightRoomObject)
        {
            this.m_LightRoomObject = lightRoomObject;
            this.m_LightRoomObject.Deleted += this.OnDeleted;
        }

        public Color LightColor
        {
            get
            {
                return Color.White;
            }
        }

        public float LightDistance
        {
            get
            {
                return 30;
            }
        }

        public Vector3 LightPosition
        {
            get
            {
                return new Vector3(this.X, this.Y, this.Z);
            }
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_LastGameContext = gameContext;
        }

        private void OnDeleted(object sender, EventArgs e)
        {
            if (this.m_LastGameContext != null)
            {
                this.m_LastGameContext.World.Entities.Remove(this);
            }
        }
    }
}