namespace Mir
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class LightEntity : IEntity
    {
        private ModelAsset m_SphereModel;

        public LightEntity(IAssetManagerProvider assetManagerProvider)
        {
            var assetManager = assetManagerProvider.GetAssetManager();

            this.m_SphereModel = assetManager.Get<ModelAsset>("effect.Protogame.DeferredLighting.Sphere");
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            var deferredRenderer = ((DeferredLighting3DWorldManager)gameContext.WorldManager).DeferredRenderer;

            deferredRenderer.AddPointLight(
                new Vector3(this.X, this.Y, this.Z), 
                Color.White,
                35f,
                1f);

            var oldWorld = renderContext.World;

            renderContext.World = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(new Vector3(this.X, this.Y, this.Z));

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                var animation = this.m_SphereModel.AvailableAnimations[Animation.AnimationNullName];
                var frame = animation.Frames[0];

                frame.LoadBuffers(renderContext.GraphicsDevice);

                renderContext.GraphicsDevice.Indices = frame.IndexBuffer;
                renderContext.GraphicsDevice.SetVertexBuffer(frame.VertexBuffer);

                renderContext.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    frame.VertexBuffer.VertexCount,
                    0,
                    frame.IndexBuffer.IndexCount / 3);
            }

            renderContext.World = oldWorld;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var pos = new Vector3(20, 14, 20);
            pos += Vector3.Transform(new Vector3(-5, 0, 0), Matrix.CreateRotationY(gameContext.FrameCount / 100f));

            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
        }
    }
}
