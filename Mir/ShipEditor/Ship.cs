namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class Ship
    {
        private readonly IFactory m_Factory;

        private readonly TextureAsset m_TextureAsset;

        private bool m_BuffersNeedRecalculation;

        private IndexBuffer m_CulledIndexBuffer;

        private VertexBuffer m_CulledVertexBuffer;

        private IndexBuffer m_FullIndexBuffer;

        private VertexBuffer m_FullVertexBuffer;

        private int m_MaximumCullY;

        public Ship(IAssetManagerProvider assetManagerProvider, IFactory factory)
        {
            this.m_Factory = factory;
            this.Cells = new List<ShipCell>();
            this.Rooms = new List<Room>();
            this.m_CulledVertexBuffer = null;
            this.m_CulledIndexBuffer = null;
            this.m_BuffersNeedRecalculation = true;

            this.m_TextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");
        }

        public List<ShipCell> Cells { get; set; }

        public int MaximumX { get; set; }

        public int MaximumY { get; set; }

        public int MaximumZ { get; set; }

        public int MinimumX { get; set; }

        public int MinimumY { get; set; }

        public int MinimumZ { get; set; }

        public List<Room> Rooms { get; set; }

        public void ClearCell(int x, int y, int z)
        {
            this.Cells.RemoveAll(a => a.X == x && a.Y == y && a.Z == z && a.Room == null);

            this.m_BuffersNeedRecalculation = true;
        }

        public void CreateRoom(int x, int y, int z, int width, int height, int depth)
        {
            for (var ix = x; ix < x + width; ix++)
            {
                for (var iy = y; iy < y + height; iy++)
                {
                    for (var iz = z; iz < z + depth; iz++)
                    {
                        if (!this.Cells.Any(a => a.X == x && a.Y == y && a.Z == z && a.Room == null))
                        {
                            return;
                        }
                    }
                }
            }

            this.Cells.RemoveAll(
                a => a.X >= x && a.Y >= y && a.Z >= z && a.X < x + width && a.Y < y + height && a.Z < z + depth);

            var room = this.m_Factory.CreateRoom();
            room.X = x * 10;
            room.Y = y * 10;
            room.Z = z * 10;
            room.Width = width * 10;
            room.Height = height * 10;
            room.Depth = depth * 10;

            for (var ix = x; ix < x + width; ix++)
            {
                for (var iy = y; iy < y + height; iy++)
                {
                    for (var iz = z; iz < z + depth; iz++)
                    {
                        this.Cells.Add(
                            new ShipCell
                            {
                                X = ix, 
                                Y = iy, 
                                Z = iz, 
                                Room = room, 
                                AboveTextureIndex = 11, 
                                BelowTextureIndex = 11, 
                                LeftTextureIndex = 11, 
                                RightTextureIndex = 11, 
                                FrontTextureIndex = 11, 
                                BackTextureIndex = 11, 
                            });
                    }
                }
            }

            this.Rooms.Add(room);

            this.m_BuffersNeedRecalculation = true;
        }

        public void DeleteRoom(Room room)
        {
            this.Cells.RemoveAll(a => a.Room == room);

            for (var ix = room.X / 10; ix < (room.X + room.Width) / 10; ix++)
            {
                for (var iy = room.Y / 10; iy < (room.Y + room.Height) / 10; iy++)
                {
                    for (var iz = room.Z / 10; iz < (room.Z + room.Depth) / 10; iz++)
                    {
                        this.Cells.Add(new ShipCell { X = (int)ix, Y = (int)iy, Z = (int)iz, });
                    }
                }
            }

            this.Rooms.Remove(room);

            this.m_BuffersNeedRecalculation = true;
        }

        public void FillCell(int x, int y, int z)
        {
            if (this.Cells.Any(a => a.X == x && a.Y == y && a.Z == z))
            {
                return;
            }

            this.Cells.Add(new ShipCell { X = x, Y = y, Z = z });

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

            foreach (var room in this.Rooms)
            {
                var matrix = Matrix.CreateScale(0.1f) * Matrix.CreateTranslation(room.X / 10, room.Y / 10, room.Z / 10);

                room.Render(gameContext, renderContext, null, false, matrix);
            }
        }

        public void SetVerticalVisibilityCull(int maxY)
        {
            this.m_MaximumCullY = maxY;

            this.m_BuffersNeedRecalculation = true;
        }

        private void RecalculateBuffers(IRenderContext renderContext)
        {
            var fullVertexes = new List<VertexPositionNormalTexture>();
            var fullIndicies = new List<int>();
            var culledVertexes = new List<VertexPositionNormalTexture>();
            var culledIndicies = new List<int>();

            var cellDictionary = this.Cells.ToDictionary(x => new Vector3(x.X, x.Y, x.Z), x => x);

            foreach (var kv in cellDictionary)
            {
                var pos = kv.Key;
                var cell = kv.Value;

                if (cell.Room != null)
                {
                    // Rooms are rendered with their actual logic.
                    continue;
                }

                var hasAbove = cellDictionary.ContainsKey(pos + Vector3.Up) && cell.Y != this.m_MaximumCullY;
                var hasBelow = cellDictionary.ContainsKey(pos + Vector3.Down);
                var hasLeft = cellDictionary.ContainsKey(pos + Vector3.Left);
                var hasRight = cellDictionary.ContainsKey(pos + Vector3.Right);
                var hasForward = cellDictionary.ContainsKey(pos + Vector3.Backward);
                var hasBackward = cellDictionary.ContainsKey(pos + Vector3.Forward);

                var newVertexes = cell.CalculateVertexPositionNormalTextures();
                var newIndicies = cell.CalculateMeshIndicies(hasAbove, hasBelow, hasLeft, hasRight, hasForward, hasBackward);

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