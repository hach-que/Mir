namespace Mir
{
    using Jitter;
    using Jitter.LinearMath;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class XnaDebugDrawer : IDebugDrawer
    {
        private readonly GraphicsDevice m_GraphicsDevice;

        public XnaDebugDrawer(GraphicsDevice graphicsDevice)
        {
            this.m_GraphicsDevice = graphicsDevice;
        }

        public void DrawLine(JVector start, JVector end)
        {
        }

        public void DrawPoint(JVector pos)
        {
        }

        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
        {
            var other = this.m_GraphicsDevice.RasterizerState.CullMode;

            this.m_GraphicsDevice.RasterizerState.CullMode = CullMode.None;

            this.m_GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList, 
                new[]
                {
                    new VertexPositionNormalTexture(pos1.ToXNAVector(), Vector3.One, Vector2.Zero), 
                    new VertexPositionNormalTexture(pos2.ToXNAVector(), Vector3.One, Vector2.Zero), 
                    new VertexPositionNormalTexture(pos3.ToXNAVector(), Vector3.One, Vector2.Zero)
                }, 
                0, 
                1);

            this.m_GraphicsDevice.RasterizerState.CullMode = other;
        }
    }
}