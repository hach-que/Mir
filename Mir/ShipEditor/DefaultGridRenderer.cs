namespace Mir
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class DefaultGridRenderer : IGridRenderer
    {
        public void Render(
            IRenderContext renderContext,
            int gridX,
            int gridY,
            int gridZ,
            int horRange,
            int vertRange,
            int verticalSelection,
            Color baseColor,
            Color gridColor)
        {
            this.RenderXZ(renderContext, gridX, gridY, gridZ, horRange, baseColor, gridColor);
            this.RenderXY(renderContext, gridX, gridY, gridZ, horRange, vertRange, verticalSelection, baseColor, gridColor);
            this.RenderZY(renderContext, gridX, gridY, gridZ, horRange, vertRange, verticalSelection, baseColor, gridColor);
        }

        private void RenderXZ(
            IRenderContext renderContext,
            int gridX,
            int gridY,
            int gridZ,
            int horRange,
            Color baseColor,
            Color gridColor)
        {
            var vertexes = new List<VertexPositionColor>();
            var indicies = new List<short>();

            for (var x = gridX - horRange; x < gridX + horRange; x++)
            {
                var color = baseColor;

                if (x % 20 == 0)
                {
                    color = gridColor;
                }

                if (x > gridX + horRange - 5)
                {
                    var b = (1f / 5f) * (5 - (x - (gridX + horRange - 5)));
                    color *= b;
                }

                for (var z = 0; z < 5; z++)
                {
                    vertexes.Add(
                        new VertexPositionColor(new Vector3(x, gridY, gridZ - horRange + z), color));
                    vertexes.Add(
                        new VertexPositionColor(new Vector3(x, gridY, gridZ - horRange + z + 1), color));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                for (var z = 0; z < 5; z++)
                {
                    var b = (1f / 5f) * z;
                    var endColor = color * b;

                    vertexes.Add(
                        new VertexPositionColor(new Vector3(x, gridY, gridZ + horRange - z), endColor));
                    vertexes.Add(
                        new VertexPositionColor(new Vector3(x, gridY, gridZ + horRange - z - 1), endColor));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                vertexes.Add(
                    new VertexPositionColor(new Vector3(x, gridY, gridZ - horRange + 5), color));
                vertexes.Add(
                    new VertexPositionColor(new Vector3(x, gridY, gridZ + horRange - 5), color));
                indicies.Add((short)(vertexes.Count - 2));
                indicies.Add((short)(vertexes.Count - 1));
            }

            for (var z = gridZ - horRange; z < gridZ + horRange; z++)
            {
                var color = baseColor;

                if (z % 20 == 0)
                {
                    color = gridColor;
                }

                if (z > gridZ + horRange - 5)
                {
                    var b = (1f / 5f) * (5 - (z - (gridZ + horRange - 5)));
                    color *= b;
                }

                for (var x = 0; x < 5; x++)
                {
                    vertexes.Add(
                        new VertexPositionColor(new Vector3(gridX - horRange + x, gridY, z), color));
                    vertexes.Add(
                        new VertexPositionColor(new Vector3(gridX - horRange + x + 1, gridY, z), color));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                for (var x = 0; x < 5; x++)
                {
                    var b = (1f / 5f) * x;
                    var endColor = color * b;

                    vertexes.Add(
                        new VertexPositionColor(new Vector3(gridX + horRange - x, gridY, z), endColor));
                    vertexes.Add(
                        new VertexPositionColor(new Vector3(gridX + horRange - x - 1, gridY, z), endColor));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                vertexes.Add(
                    new VertexPositionColor(new Vector3(gridX - horRange + 5, gridY, z), color));
                vertexes.Add(
                    new VertexPositionColor(new Vector3(gridX + horRange - 5, gridY, z), color));
                indicies.Add((short)(vertexes.Count - 2));
                indicies.Add((short)(vertexes.Count - 1));
            }

            renderContext.EnableVertexColors();

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertexes.ToArray(),
                    0,
                    vertexes.Count,
                    indicies.ToArray(),
                    0,
                    indicies.Count / 2);
            }
        }

        private void RenderXY(IRenderContext renderContext, int gridX, int gridY, int gridZ, int horRange, int vertRange, int verticalSelection, Color baseColor, Color gridColor)
        {
            var vertexes = new List<VertexPositionColor>();
            var indicies = new List<short>();

            for (var x = gridX - horRange; x < gridX + horRange; x++)
            {
                var color = baseColor;

                if (x % 20 == 0)
                {
                    color = gridColor;
                }

                if (x > gridX + horRange - 5)
                {
                    var b = (1f / 5f) * (5 - (x - (gridX + horRange - 5)));
                    color *= b;
                }

                for (var y = 0; y < 5; y++)
                {
                    var b = (1f / 5f) * y;
                    var endColor = color * b;

                    vertexes.Add(new VertexPositionColor(new Vector3(x, gridY + vertRange - y, gridZ - horRange), endColor));
                    vertexes.Add(new VertexPositionColor(new Vector3(x, gridY + vertRange - y - 1, gridZ - horRange), endColor));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                vertexes.Add(new VertexPositionColor(new Vector3(x, gridY, gridZ - horRange), color));
                vertexes.Add(new VertexPositionColor(new Vector3(x, gridY + horRange - 5, gridZ - horRange), color));
                indicies.Add((short)(vertexes.Count - 2));
                indicies.Add((short)(vertexes.Count - 1));
            }

            for (var y = gridY; y < gridY + vertRange; y++)
            {
                var color = baseColor;

                if (y % 10 == 0)
                {
                    color = gridColor;
                }

                if (y == gridY + verticalSelection || y == gridY)
                {
                    color = Color.White;
                }

                if (y > gridY + vertRange - 5)
                {
                    var b = (1f / 5f) * (5 - (y - (gridY + vertRange - 5)));
                    color *= b;
                }

                for (var x = 0; x < 5; x++)
                {
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange + x, y, gridZ - horRange), color));
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange + x + 1, y, gridZ - horRange), color));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                for (var x = 0; x < 5; x++)
                {
                    var b = (1f / 5f) * x;
                    var endColor = color * b;

                    vertexes.Add(new VertexPositionColor(new Vector3(gridX + horRange - x, y, gridZ - horRange), endColor));
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX + horRange - x - 1, y, gridZ - horRange), endColor));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange + 5, y, gridZ - horRange), color));
                vertexes.Add(new VertexPositionColor(new Vector3(gridX + horRange - 5, y, gridZ - horRange), color));
                indicies.Add((short)(vertexes.Count - 2));
                indicies.Add((short)(vertexes.Count - 1));
            }
            
            renderContext.EnableVertexColors();

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertexes.ToArray(),
                    0,
                    vertexes.Count,
                    indicies.ToArray(),
                    0,
                    indicies.Count / 2);
            }
        }

        private void RenderZY(IRenderContext renderContext, int gridX, int gridY, int gridZ, int horRange, int vertRange, int verticalSelection, Color baseColor, Color gridColor)
        {
            var vertexes = new List<VertexPositionColor>();
            var indicies = new List<short>();

            for (var z = gridZ - horRange; z < gridZ + horRange; z++)
            {
                var color = baseColor;

                if (z % 20 == 0)
                {
                    color = gridColor;
                }

                if (z > gridZ + horRange - 5)
                {
                    var b = (1f / 5f) * (5 - (z - (gridZ + horRange - 5)));
                    color *= b;
                }

                for (var y = 0; y < 5; y++)
                {
                    var b = (1f / 5f) * y;
                    var endColor = color * b;

                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, gridY + vertRange - y, z), endColor));
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, gridY + vertRange - y - 1, z), endColor));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, gridY, z), color));
                vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, gridY + horRange - 5, z), color));
                indicies.Add((short)(vertexes.Count - 2));
                indicies.Add((short)(vertexes.Count - 1));
            }

            for (var y = gridY; y < gridY + vertRange; y++)
            {
                var color = baseColor;

                if (y % 10 == 0)
                {
                    color = gridColor;
                }

                if (y == gridY + verticalSelection || y == gridY)
                {
                    color = Color.White;
                }

                if (y > gridY + vertRange - 5)
                {
                    var b = (1f / 5f) * (5 - (y - (gridY + vertRange - 5)));
                    color *= b;
                }

                for (var z = 0; z < 5; z++)
                {
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, y, gridZ - horRange + z), color));
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, y, gridZ - horRange + z + 1), color));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                for (var z = 0; z < 5; z++)
                {
                    var b = (1f / 5f) * z;
                    var endColor = color * b;

                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, y, gridZ + horRange - z), endColor));
                    vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, y, gridZ + horRange - z - 1), endColor));
                    indicies.Add((short)(vertexes.Count - 2));
                    indicies.Add((short)(vertexes.Count - 1));
                }

                vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, y, gridZ - horRange + 5), color));
                vertexes.Add(new VertexPositionColor(new Vector3(gridX - horRange, y, gridZ + horRange - 5), color));
                indicies.Add((short)(vertexes.Count - 2));
                indicies.Add((short)(vertexes.Count - 1));
            }

            renderContext.EnableVertexColors();

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                renderContext.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertexes.ToArray(),
                    0,
                    vertexes.Count,
                    indicies.ToArray(),
                    0,
                    indicies.Count / 2);
            }
        }
    }
}