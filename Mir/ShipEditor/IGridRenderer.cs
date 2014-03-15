namespace Mir
{
    using Microsoft.Xna.Framework;
    using Protogame;

    public interface IGridRenderer
    {
        void Render(IRenderContext renderContext, int gridX, int gridY, int gridZ, int horRange, int vertRange, int verticalSelection, Color baseColor, Color gridColor);
    }
}