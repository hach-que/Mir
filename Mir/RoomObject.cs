namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Jitter;
    using Jitter.Collision.Shapes;
    using Jitter.Dynamics;
    using Jitter.LinearMath;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class RoomObject : IMesh
    {
        private const float AtlasRatio = AtlasSize / CellSize;

        private const float AtlasSize = 144f;

        private const float CellSize = 16f;

        private readonly int[] m_TouchAnim = { 72, 73, 74, 75, 76, 77, 78, 79, 80, 79, 78, 77, 76, 75, 74, 73 };

        private int m_AboveTextureIndex;

        private int m_BackTextureIndex;

        private int m_BelowTextureIndex;

        private int m_Depth;

        private int m_FrontTextureIndex;

        private int m_Height;

        private IndexBuffer m_IndexBuffer;

        private int m_LeftBackEdgeMode;

        private int m_LeftFrontEdgeMode;

        private int m_LeftTextureIndex;

        private bool m_PendRecalculation;

        private int m_RightBackEdgeMode;

        private int m_RightFrontEdgeMode;

        private int m_RightTextureIndex;

        private RigidBody m_RigidBody;

        private JitterWorld m_JitterWorld;

        private int m_TouchAnimIndex;

        private VertexBuffer m_VertexBuffer;

        private int m_Width;

        private int m_X;

        private int m_Y;

        private int m_Z;

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

            this.m_PendRecalculation = true;
        }

        public event EventHandler Deleted;

        public int AboveTextureIndex
        {
            get
            {
                return this.m_AboveTextureIndex;
            }

            set
            {
                if (this.m_AboveTextureIndex == value)
                {
                    return;
                }

                this.m_AboveTextureIndex = value;
                this.m_PendRecalculation = true;
            }
        }

        public int BackTextureIndex
        {
            get
            {
                return this.m_BackTextureIndex;
            }

            set
            {
                if (this.m_BackTextureIndex == value)
                {
                    return;
                }

                this.m_BackTextureIndex = value;
                this.m_PendRecalculation = true;
            }
        }

        public int BelowTextureIndex
        {
            get
            {
                return this.m_BelowTextureIndex;
            }

            set
            {
                if (this.m_BelowTextureIndex == value)
                {
                    return;
                }

                this.m_BelowTextureIndex = value;
                this.m_PendRecalculation = true;
            }
        }

        public int Depth
        {
            get
            {
                return this.m_Depth;
            }

            set
            {
                if (this.m_Depth == value)
                {
                    return;
                }

                this.m_Depth = value;
                this.m_PendRecalculation = true;
            }
        }

        public int FrontTextureIndex
        {
            get
            {
                return this.m_FrontTextureIndex;
            }

            set
            {
                if (this.m_FrontTextureIndex == value)
                {
                    return;
                }

                this.m_FrontTextureIndex = value;
                this.m_PendRecalculation = true;
            }
        }

        public int Height
        {
            get
            {
                return this.m_Height;
            }

            set
            {
                if (this.m_Height == value)
                {
                    return;
                }

                this.m_Height = value;
                this.m_PendRecalculation = true;
            }
        }

        public int LeftBackEdgeMode
        {
            get
            {
                return this.m_LeftBackEdgeMode;
            }

            set
            {
                if (this.m_LeftBackEdgeMode == value)
                {
                    return;
                }

                this.m_LeftBackEdgeMode = value;
                this.m_PendRecalculation = true;
            }
        }

        public int LeftFrontEdgeMode
        {
            get
            {
                return this.m_LeftFrontEdgeMode;
            }

            set
            {
                if (this.m_LeftFrontEdgeMode == value)
                {
                    return;
                }

                this.m_LeftFrontEdgeMode = value;
                this.m_PendRecalculation = true;
            }
        }

        public int LeftTextureIndex
        {
            get
            {
                return this.m_LeftTextureIndex;
            }

            set
            {
                if (this.m_LeftTextureIndex == value)
                {
                    return;
                }

                this.m_LeftTextureIndex = value;
                this.m_PendRecalculation = true;
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

        public int RightBackEdgeMode
        {
            get
            {
                return this.m_RightBackEdgeMode;
            }

            set
            {
                if (this.m_RightBackEdgeMode == value)
                {
                    return;
                }

                this.m_RightBackEdgeMode = value;
                this.m_PendRecalculation = true;
            }
        }

        public int RightFrontEdgeMode
        {
            get
            {
                return this.m_RightFrontEdgeMode;
            }

            set
            {
                if (this.m_RightFrontEdgeMode == value)
                {
                    return;
                }

                this.m_RightFrontEdgeMode = value;
                this.m_PendRecalculation = true;
            }
        }

        public int RightTextureIndex
        {
            get
            {
                return this.m_RightTextureIndex;
            }

            set
            {
                if (this.m_RightTextureIndex == value)
                {
                    return;
                }

                this.m_RightTextureIndex = value;
                this.m_PendRecalculation = true;
            }
        }

        public bool SizeLocked { get; set; }

        public virtual string Type
        {
            get
            {
                return "object";
            }
        }

        public int Width
        {
            get
            {
                return this.m_Width;
            }

            set
            {
                if (this.m_Width == value)
                {
                    return;
                }

                this.m_Width = value;
                this.m_PendRecalculation = true;
            }
        }

        public int X
        {
            get
            {
                return this.m_X;
            }

            set
            {
                if (this.m_X == value)
                {
                    return;
                }

                this.m_X = value;
                this.m_PendRecalculation = true;
            }
        }

        public int Y
        {
            get
            {
                return this.m_Y;
            }

            set
            {
                if (this.m_Y == value)
                {
                    return;
                }

                this.m_Y = value;
                this.m_PendRecalculation = true;
            }
        }

        public int Z
        {
            get
            {
                return this.m_Z;
            }

            set
            {
                if (this.m_Z == value)
                {
                    return;
                }

                this.m_Z = value;
                this.m_PendRecalculation = true;
            }
        }

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
            var x = (float)(idx % (int)AtlasRatio);
            // ReSharper disable once PossibleLossOfFraction
            var y = (float)(idx / (int)AtlasRatio);
            x = (x * CellSize) / AtlasSize;
            y = (y * CellSize) / AtlasSize;

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
            if (this.m_JitterWorld != null && this.m_RigidBody != null)
            {
                this.m_JitterWorld.RemoveBody(this.m_RigidBody);
                this.m_JitterWorld = null;
                this.m_RigidBody = null;
            }

            if (this.Deleted != null)
            {
                this.Deleted(this, new EventArgs());
            }
        }

        public virtual void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_PendRecalculation)
            {
                this.RecalculateObject(gameContext, renderContext);
            }

            renderContext.GraphicsDevice.Indices = this.m_IndexBuffer;
            renderContext.GraphicsDevice.SetVertexBuffer(this.m_VertexBuffer);

            renderContext.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList, 
                0, 
                0, 
                this.m_VertexBuffer.VertexCount, 
                0, 
                this.m_IndexBuffer.IndexCount / 3);

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
            }

            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
        }

        public void RenderVerticalEdge(IRenderContext renderContext, int verticalEdge)
        {
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

        private IEnumerable<Vector3> GetVertexes()
        {
            var matrix = Matrix.CreateScale(this.Width, this.Height, this.Depth)
                         * Matrix.CreateTranslation(this.X, this.Y, this.Z);

            var leftBackBelowMod = this.LeftBackEdgeMode == 2 ? 1 : 0;
            var leftBackAboveMod = this.LeftBackEdgeMode == 1 ? -1 : 0;
            var leftFrontBelowMod = this.LeftFrontEdgeMode == 2 ? 1 : 0;
            var leftFrontAboveMod = this.LeftFrontEdgeMode == 1 ? -1 : 0;

            var rightBackBelowMod = this.RightBackEdgeMode == 2 ? 1 : 0;
            var rightBackAboveMod = this.RightBackEdgeMode == 1 ? -1 : 0;
            var rightFrontBelowMod = this.RightFrontEdgeMode == 2 ? 1 : 0;
            var rightFrontAboveMod = this.RightFrontEdgeMode == 1 ? -1 : 0;

            return new[]
            {
                Vector3.Transform(new Vector3(0, 0 + leftBackBelowMod, 0), matrix), 
                Vector3.Transform(new Vector3(0, 0 + leftFrontBelowMod, 1), matrix), 
                Vector3.Transform(new Vector3(0, 1 + leftBackAboveMod, 0), matrix), 
                Vector3.Transform(new Vector3(0, 1 + leftFrontAboveMod, 1), matrix), 
                Vector3.Transform(new Vector3(1, 0 + rightBackBelowMod, 0), matrix), 
                Vector3.Transform(new Vector3(1, 0 + rightFrontBelowMod, 1), matrix), 
                Vector3.Transform(new Vector3(1, 1 + rightBackAboveMod, 0), matrix), 
                Vector3.Transform(new Vector3(1, 1 + rightFrontAboveMod, 1), matrix)
            };
        }

        private void RecalculateObject(IGameContext gameContext, IRenderContext renderContext)
        {
            // Recalculate vertex and index buffers.
            if (this.m_IndexBuffer != null)
            {
                this.m_IndexBuffer.Dispose();
            }

            if (this.m_VertexBuffer != null)
            {
                this.m_VertexBuffer.Dispose();
            }

            this.m_IndexBuffer = new IndexBuffer(
                renderContext.GraphicsDevice,
                typeof(short), 
                this.MeshIndicies.Length, 
                BufferUsage.None);
            this.m_IndexBuffer.SetData(this.MeshIndicies);

            var vertexes = this.GetVertexPositionNormalTextures(false);
            this.m_VertexBuffer = new VertexBuffer(
                renderContext.GraphicsDevice, 
                typeof(VertexPositionNormalTexture), 
                vertexes.Length, 
                BufferUsage.None);
            this.m_VertexBuffer.SetData(vertexes);

            // Recalculate physics object.
            var world = gameContext.World as RoomEditorWorld;
            if (world != null)
            {
                if (this.m_RigidBody != null)
                {
                    this.m_JitterWorld.RemoveBody(this.m_RigidBody);
                }

                var shape = new ConvexHullShape(this.GetVertexes().Select(x => x.ToJitterVector()).ToList());
                this.m_JitterWorld = world.JitterWorld;
                this.m_RigidBody = new RigidBody(shape);
                this.m_RigidBody.IsStatic = true;

                // The shape vertexes include X, Y, Z position offset, so
                // the shift moves the object completely into the correct
                // position.
                this.m_RigidBody.Position = new JVector(0, 0, 0) - shape.Shift;
                this.m_JitterWorld.AddBody(this.m_RigidBody);
            }
            else
            {
                // We can't update the physics entity, so pend until we can.
                this.m_PendRecalculation = true;
            }
        }
    }

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
                    new VertexPositionNormalTexture(pos3.ToXNAVector(), Vector3.One, Vector2.Zero),
                },
                0,
                1);

            this.m_GraphicsDevice.RasterizerState.CullMode = other;
        }
    }
}