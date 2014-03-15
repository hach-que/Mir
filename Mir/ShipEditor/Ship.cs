namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class Ship
    {
        private VertexBuffer m_CulledVertexBuffer;

        private IndexBuffer m_CulledIndexBuffer;

        private VertexBuffer m_FullVertexBuffer;

        private IndexBuffer m_FullIndexBuffer;

        private bool m_BuffersNeedRecalculation;

        private readonly TextureAsset m_TextureAsset;

        private int m_MaximumCullY;

        public Ship(IAssetManagerProvider assetManagerProvider)
        {
            this.Cells = new List<ShipCell>();
            this.m_CulledVertexBuffer = null;
            this.m_CulledIndexBuffer = null;
            this.m_BuffersNeedRecalculation = true;

            this.m_TextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");
        }

        public int MinimumX { get; set; }

        public int MaximumX { get; set; }

        public int MinimumY { get; set; }

        public int MaximumY { get; set; }

        public int MinimumZ { get; set; }
        
        public int MaximumZ { get; set; }

        public List<ShipCell> Cells { get; set; }

        public void FillCell(int x, int y, int z)
        {
            if (this.Cells.Any(a => a.X == x && a.Y == y && a.Z == z))
            {
                return;
            }

            this.Cells.Add(new ShipCell
            {
                X = x,
                Y = y,
                Z = z
            });

            this.m_BuffersNeedRecalculation = true;
        }

        public void ClearCell(int x, int y, int z)
        {
            this.Cells.RemoveAll(a => a.X == x && a.Y == y && a.Z == z);

            this.m_BuffersNeedRecalculation = true;
        }

        public void SetVerticalVisibilityCull(int maxY)
        {
            this.m_MaximumCullY = maxY;

            this.m_BuffersNeedRecalculation = true;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext, bool renderFull = false)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            if (this.m_BuffersNeedRecalculation)
            {
                this.RecalculateBuffers(renderContext);
            }

            if (renderFull)
            {
                if (this.m_FullVertexBuffer != null && this.m_FullIndexBuffer != null)
                {
                    renderContext.World = Matrix.Identity;

                    renderContext.EnableTextures();
                    renderContext.SetActiveTexture(this.m_TextureAsset.Texture);

                    foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        renderContext.GraphicsDevice.Indices = this.m_FullIndexBuffer;
                        renderContext.GraphicsDevice.SetVertexBuffer(this.m_FullVertexBuffer);

                        renderContext.GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            0,
                            0,
                            this.m_FullVertexBuffer.VertexCount,
                            0,
                            this.m_FullIndexBuffer.IndexCount / 3);
                    }
                }
            }
            else
            {
                if (this.m_CulledVertexBuffer != null && this.m_CulledIndexBuffer != null)
                {
                    renderContext.World = Matrix.Identity;

                    renderContext.EnableTextures();
                    renderContext.SetActiveTexture(this.m_TextureAsset.Texture);

                    foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        renderContext.GraphicsDevice.Indices = this.m_CulledIndexBuffer;
                        renderContext.GraphicsDevice.SetVertexBuffer(this.m_CulledVertexBuffer);

                        renderContext.GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            0,
                            0,
                            this.m_CulledVertexBuffer.VertexCount,
                            0,
                            this.m_CulledIndexBuffer.IndexCount / 3);
                    }
                }
            }
        }

        private void RecalculateBuffers(IRenderContext renderContext)
        {
            var fullVertexes = new List<VertexPositionNormalTexture>();
            var fullIndicies = new List<int>();
            var culledVertexes = new List<VertexPositionNormalTexture>();
            var culledIndicies = new List<int>();
            foreach (var cell in this.Cells)
            {
                var newVertexes = cell.CalculateVertexPositionNormalTextures();
                var newIndicies = cell.CalculateMeshIndicies();

                fullIndicies.AddRange(newIndicies.Select(x => x + fullVertexes.Count));
                fullVertexes.AddRange(newVertexes);

                if (cell.Y > this.m_MaximumCullY)
                {
                    // Not visible.
                    continue;
                }

                culledIndicies.AddRange(newIndicies.Select(x => x + culledVertexes.Count));
                culledVertexes.AddRange(newVertexes);
            }

            if (this.m_CulledVertexBuffer != null)
            {
                this.m_CulledVertexBuffer.Dispose();
                this.m_CulledVertexBuffer = null;
            }

            if (this.m_CulledIndexBuffer != null)
            {
                this.m_CulledIndexBuffer.Dispose();
                this.m_CulledIndexBuffer = null;
            }

            if (this.m_FullVertexBuffer != null)
            {
                this.m_FullVertexBuffer.Dispose();
                this.m_FullVertexBuffer = null;
            }

            if (this.m_FullIndexBuffer != null)
            {
                this.m_FullIndexBuffer.Dispose();
                this.m_FullIndexBuffer = null;
            }

            if (culledVertexes.Count != 0)
            {
                this.m_CulledVertexBuffer = new VertexBuffer(
                    renderContext.GraphicsDevice,
                    VertexPositionNormalTexture.VertexDeclaration,
                    culledVertexes.Count,
                    BufferUsage.None);
                this.m_CulledVertexBuffer.SetData(culledVertexes.ToArray());
                this.m_CulledIndexBuffer = new IndexBuffer(
                    renderContext.GraphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    culledIndicies.Count,
                    BufferUsage.None);
                this.m_CulledIndexBuffer.SetData(culledIndicies.ToArray());
            }

            if (fullVertexes.Count != 0)
            {
                this.m_FullVertexBuffer = new VertexBuffer(
                    renderContext.GraphicsDevice,
                    VertexPositionNormalTexture.VertexDeclaration,
                    fullVertexes.Count,
                    BufferUsage.None);
                this.m_FullVertexBuffer.SetData(fullVertexes.ToArray());
                this.m_FullIndexBuffer = new IndexBuffer(
                    renderContext.GraphicsDevice,
                    IndexElementSize.ThirtyTwoBits,
                    fullIndicies.Count,
                    BufferUsage.None);
                this.m_FullIndexBuffer.SetData(fullIndicies.ToArray());
            }

            this.m_BuffersNeedRecalculation = false;
        }
    }
}