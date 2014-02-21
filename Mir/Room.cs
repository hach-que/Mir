using System;
using System.Collections.Generic;
using System.Linq;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mir
{
    /// <summary>
    /// http://www.0x10cportal.com/460-0x10c-room-editor-demo
    /// 
    /// A room is made of 6 walls facing inward and a series of objects that are
    /// used to shape the walls.  The room can be of any size, but adheres to a
    /// grid both for it's shape and for the shape of objects internally (such that
    /// it is easy for players to modify objects).
    ///  
    /// Each object is a cube, and players can interact with them to change the
    /// dimension of any axis, move the cube, or flag corners as omitted to
    /// cause the object to form diagonal shapes.
    /// </summary>
    public class Room : IArea, IMesh
    {
        private readonly IEnumerable<RoomObject> m_RoomObjects;

        private readonly TextureAsset m_TextureAsset;

        public Room(IAssetManagerProvider assetManagerProvider)
        {
            this.FrontTextureIndex = 9;
            this.BackTextureIndex = 9;
            this.LeftTextureIndex = 9;
            this.RightTextureIndex = 9;
            this.AboveTextureIndex = 2;
            this.BelowTextureIndex = 1;

            this.Width = 40;
            this.Height = 20;
            this.Depth = 50;

            this.m_TextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");
        }

        public float X { get; set; }

        public float Y { get; set; } 

        public float Z { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public int FrontTextureIndex { get; set; }
        public int BackTextureIndex { get; set; }
        public int LeftTextureIndex { get; set; }
        public int RightTextureIndex { get; set; }
        public int AboveTextureIndex { get; set; }
        public int BelowTextureIndex { get; set; }

        private static float m_AtlasSize = 144f;

        private static float m_CellSize = 16f;

        private static float m_AtlasRatio = m_AtlasSize / m_CellSize;

        public Vector2 GetTopLeftTextureUV(int idx)
        {
            var x = (float)(idx % (int)m_AtlasRatio);
            var y = (float)(idx / (int)m_AtlasRatio);
            x = (x * m_CellSize) / m_AtlasSize;
            y = (y * m_CellSize) / m_AtlasSize;

            return new Vector2(x, y);
        }

        public Vector2 GetBottomRightTextureUV(int idx)
        {
            var x = (float)(idx % (int)m_AtlasRatio);
            var y = (float)(idx / (int)m_AtlasRatio);
            x = (x * m_CellSize) / m_AtlasSize;
            y = (y * m_CellSize) / m_AtlasSize;
            x += 1f / m_AtlasRatio;
            y += 1f / m_AtlasRatio;

            return new Vector2(x, y);
        }

        public void Render(IRenderContext renderContext)
        {
            var oldWorld = renderContext.World;

            renderContext.World = Matrix.Identity;

            renderContext.EnableTextures();
            renderContext.SetActiveTexture(this.m_TextureAsset.Texture);

            var basicEffect = (BasicEffect)renderContext.Effect;
            basicEffect.PreferPerPixelLighting = true;
            basicEffect.LightingEnabled = true;
            basicEffect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1f, 1f);
            basicEffect.DirectionalLight0.Direction = new Vector3(0.7f, 0.8f, 0.9f);
            basicEffect.DirectionalLight0.SpecularColor = Vector3.Zero;

            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                this.RenderRoom(renderContext);
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
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(-1, 0, 0), new Vector2(leftBottomRightUV.X, leftBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(-1, 0, 0), new Vector2(leftTopLeftUV.X, leftBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(-1, 0, 0), new Vector2(leftBottomRightUV.X, leftTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(-1, 0, 0), new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(1, 0, 0), new Vector2(rightBottomRightUV.X, rightBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(1, 0, 0), new Vector2(rightTopLeftUV.X, rightBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(1, 0, 0), new Vector2(rightBottomRightUV.X, rightTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(1, 0, 0), new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(0, -1, 0), new Vector2(belowTopLeftUV.X, belowTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(0, -1, 0), new Vector2(belowTopLeftUV.X, belowBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(0, 1, 0), new Vector2(aboveTopLeftUV.X, aboveTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(0, 1, 0), new Vector2(aboveTopLeftUV.X, aboveBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(0, -1, 0), new Vector2(belowBottomRightUV.X, belowTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(0, -1, 0), new Vector2(belowBottomRightUV.X, belowBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(0, 1, 0), new Vector2(aboveBottomRightUV.X, aboveTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(0, 1, 0), new Vector2(aboveBottomRightUV.X, aboveBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(0, 0, -1), new Vector2(backTopLeftUV.X, backBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontTopLeftUV.X, frontBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(0, 0, -1), new Vector2(backTopLeftUV.X, backTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontTopLeftUV.X, frontTopLeftUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(0, 0, -1), new Vector2(backBottomRightUV.X, backBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontBottomRightUV.X, frontBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(0, 0, -1), new Vector2(backBottomRightUV.X, backTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontBottomRightUV.X, frontTopLeftUV.Y)),
            };
        }

        public Vector3[] MeshVertexPositions
        {
            get
            {
                return this.GetVertexPositionNormalTextures().Select(x => x.Position).ToArray();
            }
        }

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
                indiciesList.AddRange(new short[] { 0 + 8, 1 + 8, 4 + 8, 5 + 8, 4 + 8, 1 + 8, 0 + 8, 4 + 8, 1 + 8, 5 + 8, 1 + 8, 4 + 8 });

                // Above
                indiciesList.AddRange(new short[] { 2 + 8, 6 + 8, 3 + 8, 7 + 8, 3 + 8, 6 + 8, 2 + 8, 3 + 8, 6 + 8, 7 + 8, 6 + 8, 3 + 8 });

                // Back
                indiciesList.AddRange(new short[] { 0 + 16, 4 + 16, 2 + 16, 6 + 16, 2 + 16, 4 + 16, 0 + 16, 2 + 16, 4 + 16, 6 + 16, 4 + 16, 2 + 16 });

                // Front
                indiciesList.AddRange(new short[] { 1 + 16, 3 + 16, 5 + 16, 7 + 16, 5 + 16, 3 + 16, 1 + 16, 5 + 16, 3 + 16, 7 + 16, 3 + 16, 5 + 16 });

                var indicies = indiciesList.ToArray();
                return indicies;
            }
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

