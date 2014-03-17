namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    /// <summary>
    /// http://www.0x10cportal.com/460-0x10c-room-editor-demo
    /// A room is made of 6 walls facing inward and a series of objects that are
    /// used to shape the walls.  The room can be of any size, but adheres to a
    /// grid both for it's shape and for the shape of objects internally (such that
    /// it is easy for players to modify objects).
    /// Each object is a cube, and players can interact with them to change the
    /// dimension of any axis, move the cube, or flag corners as omitted to
    /// cause the object to form diagonal shapes.
    /// </summary>
    public class Room : IArea, IMesh
    {
        private const float AtlasRatio = AtlasSize / CellSize;

        private const float AtlasSize = 144f;

        private const float CellSize = 16f;

        private readonly List<RoomObject> m_RoomObjects;

        private readonly TextureAsset m_TextureAsset;

        private List<short> m_CachedIndicies;

        private List<VertexPositionNormalTexture> m_CachedVertexes;

        private Vector3[] m_CachedVertexPositionsArray;

        private short[] m_CachedIndexArray;

        private VertexBuffer m_VertexBuffer;

        private IndexBuffer m_IndexBuffer;

        private bool m_RefreshBuffers;

        public Room(IAssetManagerProvider assetManagerProvider)
        {
            this.FrontTextureIndex = 9;
            this.BackTextureIndex = 9;
            this.LeftTextureIndex = 9;
            this.RightTextureIndex = 9;
            this.AboveTextureIndex = 2;
            this.BelowTextureIndex = 1;

            this.Width = 80;
            this.Height = 60;
            this.Depth = 100;

            this.m_TextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");

            this.m_RoomObjects = new List<RoomObject>();
            this.m_RefreshBuffers = true;
        }

        public int AboveTextureIndex { get; set; }

        public int BackTextureIndex { get; set; }

        public int BelowTextureIndex { get; set; }

        public int Depth { get; set; }

        public int FrontTextureIndex { get; set; }

        public int Height { get; set; }

        public int LeftTextureIndex { get; set; }

        public short[] MeshIndicies
        {
            get
            {
                return this.m_CachedIndexArray;
            }
        }

        public Vector3[] MeshVertexPositions
        {
            get
            {
                return this.m_CachedVertexPositionsArray;
            }
        }

        public List<RoomObject> Objects
        {
            get
            {
                return this.m_RoomObjects;
            }
        }

        public int RightTextureIndex { get; set; }

        public Ship Ship { get; set; }

        public int Width { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Vector2 GetBottomRightTextureUV(int idx)
        {
            var x = (float)(idx % (int)AtlasRatio);

            // ReSharper disable once PossibleLossOfFraction
            var y = (float)(idx / (int)AtlasRatio);
            x = (x * CellSize) / AtlasSize;
            y = (y * CellSize) / AtlasSize;
            x += 1f / AtlasRatio;
            y += 1f / AtlasRatio;

            return new Vector2(x, y);
        }

        public Vector2 GetTopLeftTextureUV(int idx)
        {
            var x = (float)(idx % (int)AtlasRatio);

            // ReSharper disable once PossibleLossOfFraction
            var y = (float)(idx / (int)AtlasRatio);
            x = (x * CellSize) / AtlasSize;
            y = (y * CellSize) / AtlasSize;

            return new Vector2(x, y);
        }

        public void RecalculateMesh()
        {
            var vertexes = new List<VertexPositionNormalTexture>();
            var indicies = new List<short>();

            var cellDictionary = this.Ship.Cells.ToDictionary(x => new Vector3(x.X, x.Y, x.Z), x => x);

            for (var y = 0; y < this.Height; y += 10)
            {
                for (var z = 0; z < this.Depth; z += 10)
                {
                    var neighbour = new Vector3((this.X / 10) - 1, (this.Y + y) / 10, (this.Z + z) / 10);
                    if (cellDictionary.ContainsKey(neighbour) &&
                        cellDictionary[neighbour].Room == null)
                    {
                        this.AddLeftWall(new Vector3(0, y, z), vertexes, indicies);
                    }
                }
            }

            for (var y = 0; y < this.Height; y += 10)
            {
                for (var z = 0; z < this.Depth; z += 10)
                {
                    var neighbour = new Vector3((this.X + this.Width) / 10, (this.Y + y) / 10, (this.Z + z) / 10);
                    if (cellDictionary.ContainsKey(neighbour) &&
                        cellDictionary[neighbour].Room == null)
                    {
                        this.AddRightWall(new Vector3(this.Width - 10, y, z), vertexes, indicies);
                    }
                }
            }

            for (var z = 0; z < this.Depth; z += 10)
            {
                for (var x = 0; x < this.Width; x += 10)
                {
                    var neighbour = new Vector3((this.X + x) / 10, (this.Y / 10) - 1, (this.Z + z) / 10);
                    if (cellDictionary.ContainsKey(neighbour) &&
                        cellDictionary[neighbour].Room == null)
                    {
                        this.AddBelowWall(new Vector3(x, 0, z), vertexes, indicies);
                    }
                }
            }

            for (var z = 0; z < this.Depth; z += 10)
            {
                for (var x = 0; x < this.Width; x += 10)
                {
                    var neighbour = new Vector3((this.X + x) / 10, (this.Y + this.Height) / 10, (this.Z + z) / 10);
                    if (cellDictionary.ContainsKey(neighbour) &&
                        cellDictionary[neighbour].Room == null)
                    {
                        this.AddAboveWall(new Vector3(x, this.Height - 10, z), vertexes, indicies);
                    }
                }
            }

            for (var y = 0; y < this.Height; y += 10)
            {
                for (var x = 0; x < this.Width; x += 10)
                {
                    var neighbour = new Vector3((this.X + x) / 10, (this.Y + y) / 10, (this.Z / 10) - 1);
                    if (cellDictionary.ContainsKey(neighbour) &&
                        cellDictionary[neighbour].Room == null)
                    {
                        this.AddBackWall(new Vector3(x, y, 0), vertexes, indicies);
                    }
                }
            }

            for (var y = 0; y < this.Height; y += 10)
            {
                for (var x = 0; x < this.Width; x += 10)
                {
                    var neighbour = new Vector3((this.X + x) / 10, (this.Y + y) / 10, (this.Z + this.Depth) / 10);
                    if (cellDictionary.ContainsKey(neighbour) && cellDictionary[neighbour].Room == null)
                    {
                        this.AddFrontWall(new Vector3(x, y, this.Depth - 10), vertexes, indicies);
                    }
                }
            }

            this.m_CachedIndicies = indicies;
            this.m_CachedVertexes = vertexes;

            this.m_CachedIndexArray = indicies.ToArray();
            this.m_CachedVertexPositionsArray = vertexes.Select(x => x.Position).ToArray();

            this.m_RefreshBuffers = true;
        }

        public void Render(
            IGameContext gameContext, 
            IRenderContext renderContext, 
            RoomObject focused, 
            bool renderFocusedTransparently, 
            Matrix? matrix = null)
        {
            if (this.m_RefreshBuffers)
            {
                this.RefreshBuffers(renderContext);
                this.m_RefreshBuffers = false;
            }

            var oldWorld = renderContext.World;

            renderContext.World = matrix ?? Matrix.Identity;

            renderContext.EnableTextures();
            renderContext.SetActiveTexture(this.m_TextureAsset.Texture);

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this.RenderRoom(renderContext);

                foreach (var obj in this.m_RoomObjects)
                {
                    if (renderFocusedTransparently && obj == focused)
                    {
                        continue;
                    }

                    obj.Render(gameContext, renderContext);
                }
            }

            if (renderFocusedTransparently && focused != null && this.m_RoomObjects.Contains(focused))
            {
                renderContext.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                renderContext.Effect.Parameters["Alpha"].SetValue(0.5f);

                foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    focused.Render(gameContext, renderContext);
                }

                renderContext.Effect.Parameters["Alpha"].SetValue(1f);
            }

            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;

            renderContext.World = oldWorld;
        }

        private void RefreshBuffers(IRenderContext renderContext)
        {
            if (this.m_VertexBuffer != null)
            {
                this.m_VertexBuffer.Dispose();
            }

            if (this.m_IndexBuffer != null)
            {
                this.m_IndexBuffer.Dispose();
            }

            this.m_VertexBuffer = new VertexBuffer(
                renderContext.GraphicsDevice,
                VertexPositionNormalTexture.VertexDeclaration,
                this.m_CachedVertexes.Count,
                BufferUsage.None);
            this.m_VertexBuffer.SetData(this.m_CachedVertexes.ToArray());
            this.m_IndexBuffer = new IndexBuffer(
                renderContext.GraphicsDevice,
                IndexElementSize.SixteenBits,
                this.m_CachedIndicies.Count,
                BufferUsage.None);
            this.m_IndexBuffer.SetData(this.m_CachedIndicies.ToArray());
        }

        private void AddAboveWall(Vector3 position, List<VertexPositionNormalTexture> vertexes, List<short> indicies)
        {
            var matrix = Matrix.CreateScale(9f) + Matrix.CreateTranslation(position);

            var aboveTopLeftUV = this.GetTopLeftTextureUV(this.AboveTextureIndex);
            var aboveBottomRightUV = this.GetBottomRightTextureUV(this.AboveTextureIndex);

            indicies.AddRange(new[] { 0, 1, 2, 3, 2, 1 }.Select(x => (short)(x + vertexes.Count)));

            vertexes.AddRange(
                new[]
                {
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                        new Vector3(0, 1, 0), 
                        new Vector2(aboveTopLeftUV.X, aboveTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                        new Vector3(0, 1, 0), 
                        new Vector2(aboveTopLeftUV.X, aboveBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                        new Vector3(0, 1, 0), 
                        new Vector2(aboveBottomRightUV.X, aboveTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                        new Vector3(0, 1, 0), 
                        new Vector2(aboveBottomRightUV.X, aboveBottomRightUV.Y))
                });
        }

        private void AddBackWall(Vector3 position, List<VertexPositionNormalTexture> vertexes, List<short> indicies)
        {
            var matrix = Matrix.CreateScale(9f) + Matrix.CreateTranslation(position);

            var backTopLeftUV = this.GetTopLeftTextureUV(this.BackTextureIndex);
            var backBottomRightUV = this.GetBottomRightTextureUV(this.BackTextureIndex);

            indicies.AddRange(new[] { 0, 2, 1, 3, 1, 2, 0, 1, 2, 3, 2, 1 }.Select(x => (short)(x + vertexes.Count)));

            vertexes.AddRange(
                new[]
                {
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                        new Vector3(0, 0, -1), 
                        new Vector2(backTopLeftUV.X, backBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                        new Vector3(0, 0, -1), 
                        new Vector2(backTopLeftUV.X, backTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                        new Vector3(0, 0, -1), 
                        new Vector2(backBottomRightUV.X, backBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                        new Vector3(0, 0, -1), 
                        new Vector2(backBottomRightUV.X, backTopLeftUV.Y))
                });
        }

        private void AddBelowWall(Vector3 position, List<VertexPositionNormalTexture> vertexes, List<short> indicies)
        {
            var matrix = Matrix.CreateScale(9f) + Matrix.CreateTranslation(position);

            var belowTopLeftUV = this.GetTopLeftTextureUV(this.BelowTextureIndex);
            var belowBottomRightUV = this.GetBottomRightTextureUV(this.BelowTextureIndex);

            indicies.AddRange(new[] { 0, 2, 1, 3, 1, 2 }.Select(x => (short)(x + vertexes.Count)));

            vertexes.AddRange(
                new[]
                {
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                        new Vector3(0, -1, 0), 
                        new Vector2(belowTopLeftUV.X, belowTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                        new Vector3(0, -1, 0), 
                        new Vector2(belowTopLeftUV.X, belowBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                        new Vector3(0, -1, 0), 
                        new Vector2(belowBottomRightUV.X, belowTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                        new Vector3(0, -1, 0), 
                        new Vector2(belowBottomRightUV.X, belowBottomRightUV.Y))
                });
        }

        private void AddFrontWall(Vector3 position, List<VertexPositionNormalTexture> vertexes, List<short> indicies)
        {
            var matrix = Matrix.CreateScale(9f) + Matrix.CreateTranslation(position);

            var frontTopLeftUV = this.GetTopLeftTextureUV(this.FrontTextureIndex);
            var frontBottomRightUV = this.GetBottomRightTextureUV(this.FrontTextureIndex);

            indicies.AddRange(new[] { 0, 1, 2, 3, 2, 1, 0, 2, 1, 3, 1, 2 }.Select(x => (short)(x + vertexes.Count)));

            vertexes.AddRange(
                new[]
                {
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                        new Vector3(0, 0, 1), 
                        new Vector2(frontTopLeftUV.X, frontBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                        new Vector3(0, 0, 1), 
                        new Vector2(frontTopLeftUV.X, frontTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                        new Vector3(0, 0, 1), 
                        new Vector2(frontBottomRightUV.X, frontBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                        new Vector3(0, 0, 1), 
                        new Vector2(frontBottomRightUV.X, frontTopLeftUV.Y))
                });
        }

        private void AddLeftWall(Vector3 position, List<VertexPositionNormalTexture> vertexes, List<short> indicies)
        {
            var matrix = Matrix.CreateScale(9f) + Matrix.CreateTranslation(position);

            var leftTopLeftUV = this.GetTopLeftTextureUV(this.LeftTextureIndex);
            var leftBottomRightUV = this.GetBottomRightTextureUV(this.LeftTextureIndex);

            indicies.AddRange(new[] { 0, 2, 1, 3, 1, 2, 0, 1, 2, 3, 2, 1 }.Select(x => (short)(x + vertexes.Count)));

            vertexes.AddRange(
                new[]
                {
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                        new Vector3(-1, 0, 0), 
                        new Vector2(leftBottomRightUV.X, leftBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                        new Vector3(-1, 0, 0), 
                        new Vector2(leftTopLeftUV.X, leftBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                        new Vector3(-1, 0, 0), 
                        new Vector2(leftBottomRightUV.X, leftTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                        new Vector3(-1, 0, 0), 
                        new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y))
                });
        }

        private void AddRightWall(Vector3 position, List<VertexPositionNormalTexture> vertexes, List<short> indicies)
        {
            var matrix = Matrix.CreateScale(9f) + Matrix.CreateTranslation(position);

            var rightTopLeftUV = this.GetTopLeftTextureUV(this.RightTextureIndex);
            var rightBottomRightUV = this.GetBottomRightTextureUV(this.RightTextureIndex);

            indicies.AddRange(new[] { 4, 5, 6, 7, 6, 5, 4, 6, 5, 7, 5, 6 }.Select(x => (short)(x + vertexes.Count - 4)));

            vertexes.AddRange(
                new[]
                {
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                        new Vector3(1, 0, 0), 
                        new Vector2(rightBottomRightUV.X, rightBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                        new Vector3(1, 0, 0), 
                        new Vector2(rightTopLeftUV.X, rightBottomRightUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                        new Vector3(1, 0, 0), 
                        new Vector2(rightBottomRightUV.X, rightTopLeftUV.Y)), 
                    new VertexPositionNormalTexture(
                        Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                        new Vector3(1, 0, 0), 
                        new Vector2(rightTopLeftUV.X, rightTopLeftUV.Y))
                });
        }

        private void RenderRoom(IRenderContext renderContext)
        {
            renderContext.GraphicsDevice.Indices = this.m_IndexBuffer;
            renderContext.GraphicsDevice.SetVertexBuffer(this.m_VertexBuffer);

            renderContext.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                this.m_VertexBuffer.VertexCount,
                0,
                this.m_IndexBuffer.IndexCount / 3);
        }
    }
}