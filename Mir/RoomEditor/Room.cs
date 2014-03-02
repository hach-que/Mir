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
        private const float AtlasSize = 144f;

        private const float CellSize = 16f;

        private const float AtlasRatio = AtlasSize / CellSize;

        private readonly List<RoomObject> m_RoomObjects;

        private readonly TextureAsset m_TextureAsset;

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
                var indiciesList = new List<short>();

                // Left
                indiciesList.AddRange(new short[] { 0, 2, 1, 3, 1, 2, 0, 1, 2, 3, 2, 1 });

                // Right
                indiciesList.AddRange(new short[] { 4, 5, 6, 7, 6, 5, 4, 6, 5, 7, 5, 6 });

                // Below
                indiciesList.AddRange(
                    new short[] { 0 + 8, 1 + 8, 4 + 8, 5 + 8, 4 + 8, 1 + 8, 0 + 8, 4 + 8, 1 + 8, 5 + 8, 1 + 8, 4 + 8 });

                // Above
                indiciesList.AddRange(
                    new short[] { 2 + 8, 6 + 8, 3 + 8, 7 + 8, 3 + 8, 6 + 8, 2 + 8, 3 + 8, 6 + 8, 7 + 8, 6 + 8, 3 + 8 });

                // Back
                indiciesList.AddRange(
                    new short[]
                    {
                       0 + 16, 4 + 16, 2 + 16, 6 + 16, 2 + 16, 4 + 16, 0 + 16, 2 + 16, 4 + 16, 6 + 16, 4 + 16, 2 + 16 
                    });

                // Front
                indiciesList.AddRange(
                    new short[]
                    {
                       1 + 16, 3 + 16, 5 + 16, 7 + 16, 5 + 16, 3 + 16, 1 + 16, 5 + 16, 3 + 16, 7 + 16, 3 + 16, 5 + 16 
                    });

                var indicies = indiciesList.ToArray();
                return indicies;
            }
        }

        public Vector3[] MeshVertexPositions
        {
            get
            {
                return this.GetVertexPositionNormalTextures().Select(x => x.Position).ToArray();
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

        public void Render(
            IGameContext gameContext, 
            IRenderContext renderContext, 
            RoomObject focused, 
            bool renderFocusedTransparently)
        {
            var oldWorld = renderContext.World;

            renderContext.World = Matrix.Identity;

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

                renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
                renderContext.Effect.Parameters["Alpha"].SetValue(1f);
            }

            renderContext.World = oldWorld;
        }

        private VertexPositionNormalTexture[] GetVertexPositionNormalTextures()
        {
            var matrix = Matrix.CreateScale(this.Width, this.Height, this.Depth);

            var frontTopLeftUV = this.GetTopLeftTextureUV(this.FrontTextureIndex);
            var frontBottomRightUV = this.GetBottomRightTextureUV(this.FrontTextureIndex);
            var backTopLeftUV = this.GetTopLeftTextureUV(this.BackTextureIndex);
            var backBottomRightUV = this.GetBottomRightTextureUV(this.BackTextureIndex);
            var leftTopLeftUV = this.GetTopLeftTextureUV(this.LeftTextureIndex);
            var leftBottomRightUV = this.GetBottomRightTextureUV(this.LeftTextureIndex);
            var rightTopLeftUV = this.GetTopLeftTextureUV(this.RightTextureIndex);
            var rightBottomRightUV = this.GetBottomRightTextureUV(this.RightTextureIndex);
            var aboveTopLeftUV = this.GetTopLeftTextureUV(this.AboveTextureIndex);
            var aboveBottomRightUV = this.GetBottomRightTextureUV(this.AboveTextureIndex);
            var belowTopLeftUV = this.GetTopLeftTextureUV(this.BelowTextureIndex);
            var belowBottomRightUV = this.GetBottomRightTextureUV(this.BelowTextureIndex);

            return new[]
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
                    new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y)), 
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
                    new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowTopLeftUV.X, belowTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowTopLeftUV.X, belowBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveTopLeftUV.X, aboveTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveTopLeftUV.X, aboveBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowBottomRightUV.X, belowTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowBottomRightUV.X, belowBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveBottomRightUV.X, aboveTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveBottomRightUV.X, aboveBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backTopLeftUV.X, backBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontTopLeftUV.X, frontBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backTopLeftUV.X, backTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontTopLeftUV.X, frontTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backBottomRightUV.X, backBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontBottomRightUV.X, frontBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backBottomRightUV.X, backTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontBottomRightUV.X, frontTopLeftUV.Y))
            };
        }

        private void RenderRoom(IRenderContext renderContext)
        {
            var vertexes = this.GetVertexPositionNormalTextures();
            var indicies = this.MeshIndicies;

            renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList, 
                vertexes, 
                0, 
                vertexes.Length, 
                indicies, 
                0, 
                indicies.Length / 3);
        }
    }
}