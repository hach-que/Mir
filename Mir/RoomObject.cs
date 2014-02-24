namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class RoomObject : IMesh
    {
        private const float m_AtlasRatio = m_AtlasSize / m_CellSize;

        private const float m_AtlasSize = 144f;

        private const float m_CellSize = 16f;

        private readonly int[] m_TouchAnim = { 72, 73, 74, 75, 76, 77, 78, 79, 80, 79, 78, 77, 76, 75, 74, 73 };

        private int m_TouchAnimIndex;

        public RoomObject()
        {
            this.Width = 1;
            this.Height = 1;
            this.Depth = 1;

            this.FrontTextureIndex = 0;
            this.BackTextureIndex = 0;
            this.LeftTextureIndex = 0;
            this.RightTextureIndex = 0;
            this.AboveTextureIndex = 0;
            this.BelowTextureIndex = 0;
        }

        public event EventHandler Deleted;

        public int AboveTextureIndex { get; set; }

        public int BackTextureIndex { get; set; }

        public int BelowTextureIndex { get; set; }

        public int Depth { get; set; }

        public int FrontTextureIndex { get; set; }

        public int Height { get; set; }

        public int LeftBackEdgeMode { get; set; }

        public int LeftFrontEdgeMode { get; set; }

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
                return this.GetVertexPositionNormalTextures(true).Select(x => x.Position).ToArray();
            }
        }

        public int RightBackEdgeMode { get; set; }

        public int RightFrontEdgeMode { get; set; }

        public int RightTextureIndex { get; set; }

        public bool SizeLocked { get; set; }

        public virtual string Type
        {
            get
            {
                return "object";
            }
        }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

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

        public int GetFaceForTouchPosition(Vector3 pos)
        {
            if (Math.Abs(pos.X - this.X) < 0.0001f)
            {
                return 0;
            }

            if (Math.Abs(pos.X - (this.X + this.Width)) < 0.0001f)
            {
                return 1;
            }

            if (Math.Abs(pos.Z - this.Z) < 0.0001f)
            {
                return 2;
            }

            if (Math.Abs(pos.Z - (this.Z + this.Depth)) < 0.0001f)
            {
                return 3;
            }

            if (Math.Abs(pos.Y - this.Y) < 0.0001f)
            {
                return 4;
            }

            if (Math.Abs(pos.Y - (this.Y + this.Height)) < 0.0001f)
            {
                return 5;
            }

            return -1;
        }

        public Vector2 GetTopLeftTextureUV(int idx)
        {
            var x = (float)(idx % (int)m_AtlasRatio);
            var y = (float)(idx / (int)m_AtlasRatio);
            x = (x * m_CellSize) / m_AtlasSize;
            y = (y * m_CellSize) / m_AtlasSize;

            return new Vector2(x, y);
        }

        public int GetVerticalEdge(Vector3 pos)
        {
            if (Math.Abs(pos.X - this.X) < 0.1f && Math.Abs(pos.Z - this.Z) < 0.1f)
            {
                return 0;
            }

            if (Math.Abs(pos.X - (this.X + this.Width)) < 0.1f && Math.Abs(pos.Z - this.Z) < 0.1f)
            {
                return 1;
            }

            if (Math.Abs(pos.X - this.X) < 0.1f && Math.Abs(pos.Z - (this.Z + this.Depth)) < 0.1f)
            {
                return 2;
            }

            if (Math.Abs(pos.X - (this.X + this.Width)) < 0.1f && Math.Abs(pos.Z - (this.Z + this.Depth)) < 0.1f)
            {
                return 3;
            }

            return -1;
        }

        public void OnDelete()
        {
            if (this.Deleted != null)
            {
                this.Deleted(this, new EventArgs());
            }
        }

        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            var vertexes = this.GetVertexPositionNormalTextures(false);
            var indicies = this.MeshIndicies;

            renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList, 
                vertexes, 
                0, 
                vertexes.Length, 
                indicies, 
                0, 
                indicies.Length / 3);

            this.m_TouchAnimIndex++;
            if (this.m_TouchAnimIndex >= this.m_TouchAnim.Length * 3)
            {
                this.m_TouchAnimIndex = 0;
            }
        }

        public void RenderSelection(IRenderContext renderContext, int face)
        {
            var topLeftUV = this.GetTopLeftTextureUV(this.m_TouchAnim[this.m_TouchAnimIndex / 3]);
            var bottomRightUV = this.GetBottomRightTextureUV(this.m_TouchAnim[this.m_TouchAnimIndex / 3]);

            var matrix = Matrix.CreateScale(this.Width + 0.2f, this.Height + 0.2f, this.Depth + 0.2f)
                         * Matrix.CreateTranslation(this.X - 0.1f, this.Y - 0.1f, this.Z - 0.1f);

            var touchVertexes = new[]
            {
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(bottomRightUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(topLeftUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(bottomRightUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(topLeftUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(bottomRightUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(topLeftUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(bottomRightUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(topLeftUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(topLeftUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(topLeftUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(topLeftUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(topLeftUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(bottomRightUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(bottomRightUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(bottomRightUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(bottomRightUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(topLeftUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(topLeftUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(topLeftUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(topLeftUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(bottomRightUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(bottomRightUV.X, bottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(bottomRightUV.X, topLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(bottomRightUV.X, topLeftUV.Y))
            };

            renderContext.GraphicsDevice.BlendState = BlendState.Additive;

            switch (face)
            {
                case 0:
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        touchVertexes, 
                        0, 
                        touchVertexes.Length, 
                        new short[] { 0, 2, 1, 3, 1, 2 }, 
                        0, 
                        2);
                    break;
                case 1:
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        touchVertexes, 
                        0, 
                        touchVertexes.Length, 
                        new short[] { 4, 5, 6, 7, 6, 5 }, 
                        0, 
                        2);
                    break;
                case 2:
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        touchVertexes, 
                        0, 
                        touchVertexes.Length, 
                        new short[] { 0 + 16, 4 + 16, 2 + 16, 6 + 16, 2 + 16, 4 + 16 }, 
                        0, 
                        2);
                    break;
                case 3:
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        touchVertexes, 
                        0, 
                        touchVertexes.Length, 
                        new short[] { 1 + 16, 3 + 16, 5 + 16, 7 + 16, 5 + 16, 3 + 16 }, 
                        0, 
                        2);
                    break;
                case 4:
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        touchVertexes, 
                        0, 
                        touchVertexes.Length, 
                        new short[] { 0 + 8, 1 + 8, 4 + 8, 5 + 8, 4 + 8, 1 + 8 }, 
                        0, 
                        2);
                    break;
                case 5:
                    renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        touchVertexes, 
                        0, 
                        touchVertexes.Length, 
                        new short[] { 2 + 8, 6 + 8, 3 + 8, 7 + 8, 3 + 8, 6 + 8 }, 
                        0, 
                        2);
                    break;
                default:
                    break;
            }

            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
        }

        public void RenderVerticalEdge(IRenderContext renderContext, int verticalEdge)
        {
            // TODO: Maybe render this a bit nicer?
            switch (verticalEdge)
            {
                case 0:
                    this.RenderSelection(renderContext, 0);
                    this.RenderSelection(renderContext, 2);
                    break;
                case 1:
                    this.RenderSelection(renderContext, 1);
                    this.RenderSelection(renderContext, 2);
                    break;
                case 2:
                    this.RenderSelection(renderContext, 0);
                    this.RenderSelection(renderContext, 3);
                    break;
                case 3:
                    this.RenderSelection(renderContext, 1);
                    this.RenderSelection(renderContext, 3);
                    break;
            }
        }

        private VertexPositionNormalTexture[] GetVertexPositionNormalTextures(bool hitTest)
        {
            var matrix = Matrix.CreateScale(this.Width, this.Height, this.Depth)
                         * Matrix.CreateTranslation(this.X, this.Y, this.Z);

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

            var leftBackBelowMod = hitTest ? 0 : this.LeftBackEdgeMode == 2 ? 1 : 0;
            var leftBackAboveMod = hitTest ? 0 : this.LeftBackEdgeMode == 1 ? -1 : 0;
            var leftFrontBelowMod = hitTest ? 0 : this.LeftFrontEdgeMode == 2 ? 1 : 0;
            var leftFrontAboveMod = hitTest ? 0 : this.LeftFrontEdgeMode == 1 ? -1 : 0;

            var rightBackBelowMod = hitTest ? 0 : this.RightBackEdgeMode == 2 ? 1 : 0;
            var rightBackAboveMod = hitTest ? 0 : this.RightBackEdgeMode == 1 ? -1 : 0;
            var rightFrontBelowMod = hitTest ? 0 : this.RightFrontEdgeMode == 2 ? 1 : 0;
            var rightFrontAboveMod = hitTest ? 0 : this.RightFrontEdgeMode == 1 ? -1 : 0;

            return new[]
            {
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0 + leftBackBelowMod, 0), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(leftBottomRightUV.X, leftBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0 + leftFrontBelowMod, 1), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(leftTopLeftUV.X, leftBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1 + leftBackAboveMod, 0), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(leftBottomRightUV.X, leftTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1 + leftFrontAboveMod, 1), matrix), 
                    new Vector3(-1, 0, 0), 
                    new Vector2(leftTopLeftUV.X, leftTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0 + rightBackBelowMod, 0), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(rightBottomRightUV.X, rightBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0 + rightFrontBelowMod, 1), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(rightTopLeftUV.X, rightBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1 + rightBackAboveMod, 0), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(rightBottomRightUV.X, rightTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1 + rightFrontAboveMod, 1), matrix), 
                    new Vector3(1, 0, 0), 
                    new Vector2(rightTopLeftUV.X, rightTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0 + leftBackBelowMod, 0), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowTopLeftUV.X, belowTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0 + leftFrontBelowMod, 1), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowTopLeftUV.X, belowBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1 + leftBackAboveMod, 0), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveTopLeftUV.X, aboveTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1 + leftFrontAboveMod, 1), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveTopLeftUV.X, aboveBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0 + rightBackBelowMod, 0), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowBottomRightUV.X, belowTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0 + rightFrontBelowMod, 1), matrix), 
                    new Vector3(0, -1, 0), 
                    new Vector2(belowBottomRightUV.X, belowBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1 + rightBackAboveMod, 0), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveBottomRightUV.X, aboveTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1 + rightFrontAboveMod, 1), matrix), 
                    new Vector3(0, 1, 0), 
                    new Vector2(aboveBottomRightUV.X, aboveBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0 + leftBackBelowMod, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backTopLeftUV.X, backBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 0 + leftFrontBelowMod, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontTopLeftUV.X, frontBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1 + leftBackAboveMod, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backTopLeftUV.X, backTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(0, 1 + leftFrontAboveMod, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontTopLeftUV.X, frontTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0 + rightBackBelowMod, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backBottomRightUV.X, backBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 0 + rightFrontBelowMod, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontBottomRightUV.X, frontBottomRightUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1 + rightBackAboveMod, 0), matrix), 
                    new Vector3(0, 0, -1), 
                    new Vector2(backBottomRightUV.X, backTopLeftUV.Y)), 
                new VertexPositionNormalTexture(
                    Vector3.Transform(new Vector3(1, 1 + rightFrontAboveMod, 1), matrix), 
                    new Vector3(0, 0, 1), 
                    new Vector2(frontBottomRightUV.X, frontTopLeftUV.Y))
            };
        }
    }
}