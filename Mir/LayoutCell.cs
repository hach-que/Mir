using System;
using Protogame;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Mir
{
    public class LayoutCell
    {
        private readonly I3DRenderUtilities m_3DRenderUtilities;

        public LayoutCell(I3DRenderUtilities threedRenderUtilities)
        {
            this.m_3DRenderUtilities = threedRenderUtilities;

            this.FrontTextureIndex = 9;
            this.BackTextureIndex = 9;
            this.LeftTextureIndex = 9;
            this.RightTextureIndex = 9;
            this.AboveTextureIndex = 2;
            this.BelowTextureIndex = 1;
        }

        public bool FrontOpen { get; set; }
        public bool BackOpen { get; set; }
        public bool LeftOpen { get; set; }
        public bool RightOpen { get; set; }
        public bool AboveOpen { get; set; }
        public bool BelowOpen { get; set; }

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

        public virtual void Render(IRenderContext renderContext, int i, int j, int k)
        {
            var matrix = Matrix.CreateTranslation(i, j, k);

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

            var vertexes = new[]
            {
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(-1, 0, 0), new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(-1, 0, 0), new Vector2(leftTopLeftUV.X, leftBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(-1, 0, 0), new Vector2(leftBottomRightUV.X, leftTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(-1, 0, 0), new Vector2(leftBottomRightUV.X, leftBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(1, 0, 0), new Vector2(rightTopLeftUV.X, rightTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(1, 0, 0), new Vector2(rightTopLeftUV.X, rightBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(1, 0, 0), new Vector2(rightBottomRightUV.X, rightTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(1, 0, 0), new Vector2(rightBottomRightUV.X, rightBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(0, -1, 0), new Vector2(belowTopLeftUV.X, belowTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(0, -1, 0), new Vector2(belowTopLeftUV.X, belowBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(0, 1, 0), new Vector2(aboveTopLeftUV.X, aboveTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(0, 1, 0), new Vector2(aboveTopLeftUV.X, aboveBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(0, -1, 0), new Vector2(belowBottomRightUV.X, belowTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(0, -1, 0), new Vector2(belowBottomRightUV.X, belowBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(0, 1, 0), new Vector2(aboveBottomRightUV.X, aboveTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(0, 1, 0), new Vector2(aboveBottomRightUV.X, aboveBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(0, 0, -1), new Vector2(backTopLeftUV.X, backTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontTopLeftUV.X, frontTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(0, 0, -1), new Vector2(backTopLeftUV.X, backBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontTopLeftUV.X, frontBottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(0, 0, -1), new Vector2(backBottomRightUV.X, backTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontBottomRightUV.X, frontTopLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(0, 0, -1), new Vector2(backBottomRightUV.X, backBottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(0, 0, 1), new Vector2(frontBottomRightUV.X, frontBottomRightUV.Y)),
            };

            var indiciesList = new List<short>();

            if (!this.LeftOpen)
            {
                indiciesList.AddRange(new short[] { 0, 2, 1, 3, 1, 2, 0, 1, 2, 3, 2, 1 });
            }

            if (!this.RightOpen)
            {
                indiciesList.AddRange(new short[] { 4, 5, 6, 7, 6, 5, 4, 6, 5, 7, 5, 6 });
            }

            if (!this.BelowOpen)
            {
                indiciesList.AddRange(new short[] { 0 + 8, 1 + 8, 4 + 8, 5 + 8, 4 + 8, 1 + 8, 0 + 8, 4 + 8, 1 + 8, 5 + 8, 1 + 8, 4 + 8 });
            }

            if (!this.AboveOpen)
            {
                indiciesList.AddRange(new short[] { 2 + 8, 6 + 8, 3 + 8, 7 + 8, 3 + 8, 6 + 8, 2 + 8, 3 + 8, 6 + 8, 7 + 8, 6 + 8, 3 + 8 });
            }

            if (!this.BackOpen)
            {
                indiciesList.AddRange(new short[] { 0 + 16, 4 + 16, 2 + 16, 6 + 16, 2 + 16, 4 + 16, 0 + 16, 2 + 16, 4 + 16, 6 + 16, 4 + 16, 2 + 16 });
            }

            if (!this.FrontOpen)
            {
                indiciesList.AddRange(new short[] { 1 + 16, 3 + 16, 5 + 16, 7 + 16, 5 + 16, 3 + 16, 1 + 16, 5 + 16, 3 + 16, 7 + 16, 3 + 16, 5 + 16 });
            }

            var indicies = indiciesList.ToArray();

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

