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
        }

        public bool FrontOpen { get; set; }
        public bool BackOpen { get; set; }
        public bool LeftOpen { get; set; }
        public bool RightOpen { get; set; }
        public bool AboveOpen { get; set; }
        public bool BelowOpen { get; set; }

        public virtual void Render(IRenderContext renderContext, int i, int j, int k)
        {
            var topLeftUV = new Vector2(0, 0);
            var bottomRightUV = new Vector2(1, 1);

            var matrix = Matrix.CreateTranslation(i, j, k);

            var vertexes = new[]
            {
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(-1, 0, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(-1, 0, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(-1, 0, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(-1, 0, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(1, 0, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(1, 0, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(1, 0, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(1, 0, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(0, -1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(0, 1, 0), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(0, 1, 0), new Vector2(topLeftUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(0, -1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(0, 1, 0), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(0, 1, 0), new Vector2(bottomRightUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 0), matrix), new Vector3(0, 0, -1), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 0, 1), matrix), new Vector3(0, 0, 1), new Vector2(topLeftUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 0), matrix), new Vector3(0, 0, -1), new Vector2(topLeftUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(0, 1, 1), matrix), new Vector3(0, 0, 1), new Vector2(topLeftUV.X, bottomRightUV.Y)),

                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 0), matrix), new Vector3(0, 0, -1), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 0, 1), matrix), new Vector3(0, 0, 1), new Vector2(bottomRightUV.X, topLeftUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 0), matrix), new Vector3(0, 0, -1), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
                new VertexPositionNormalTexture(Vector3.Transform(new Vector3(1, 1, 1), matrix), new Vector3(0, 0, 1), new Vector2(bottomRightUV.X, bottomRightUV.Y)),
            };

            var indiciesList = new List<short>();

            if (!this.LeftOpen)
            {
                indiciesList.AddRange(new short[] { 0, 2, 1, 3, 1, 2 });
            }

            if (!this.RightOpen)
            {
                indiciesList.AddRange(new short[] { 4, 5, 6, 7, 6, 5 });
            }

            if (!this.BelowOpen)
            {
                indiciesList.AddRange(new short[] { 0 + 8, 1 + 8, 4 + 8, 5 + 8, 4 + 8, 1 + 8 });
            }

            if (!this.AboveOpen)
            {
                indiciesList.AddRange(new short[] { 2 + 8, 6 + 8, 3 + 8, 7 + 8, 3 + 8, 6 + 8 });
            }

            if (!this.BackOpen)
            {
                indiciesList.AddRange(new short[] { 0 + 16, 4 + 16, 2 + 16, 6 + 16, 2 + 16, 4 + 16 });
            }

            if (!this.FrontOpen)
            {
                indiciesList.AddRange(new short[] { 1 + 16, 3 + 16, 5 + 16, 7 + 16, 5 + 16, 3 + 16 });
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

