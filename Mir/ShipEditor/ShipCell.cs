namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ShipCell
    {
        private const float AtlasRatio = AtlasSize / CellSize;

        private const float AtlasSize = 144f;

        private const float CellSize = 16f;

        public ShipCell()
        {
            this.AboveTextureIndex = 10;
            this.BelowTextureIndex = 10;
            this.LeftTextureIndex = 10;
            this.RightTextureIndex = 10;
            this.FrontTextureIndex = 10;
            this.BackTextureIndex = 10;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int AboveTextureIndex { get; set; }

        public int BelowTextureIndex { get; set; }

        public int LeftTextureIndex { get; set; }

        public int RightTextureIndex { get; set; }

        public int FrontTextureIndex { get; set; }

        public int BackTextureIndex { get; set; }

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

        public VertexPositionNormalTexture[] CalculateVertexPositionNormalTextures()
        {
            var matrix = Matrix.Identity * Matrix.CreateTranslation(this.X, this.Y, this.Z);

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

            var leftBackBelowMod = 0; // hitTest ? 0 : this.LeftBackEdgeMode == 2 ? 1 : 0;
            var leftBackAboveMod = 0; // hitTest ? 0 : this.LeftBackEdgeMode == 1 ? -1 : 0;
            var leftFrontBelowMod = 0; // hitTest ? 0 : this.LeftFrontEdgeMode == 2 ? 1 : 0;
            var leftFrontAboveMod = 0; // hitTest ? 0 : this.LeftFrontEdgeMode == 1 ? -1 : 0;

            var rightBackBelowMod = 0; // hitTest ? 0 : this.RightBackEdgeMode == 2 ? 1 : 0;
            var rightBackAboveMod = 0; // hitTest ? 0 : this.RightBackEdgeMode == 1 ? -1 : 0;
            var rightFrontBelowMod = 0; // hitTest ? 0 : this.RightFrontEdgeMode == 2 ? 1 : 0;
            var rightFrontAboveMod = 0; // hitTest ? 0 : this.RightFrontEdgeMode == 1 ? -1 : 0;

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

        public int[] CalculateMeshIndicies()
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

            return indiciesList.Select(x => (int)x).ToArray();
        }
    }
}