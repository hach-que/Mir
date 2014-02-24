namespace Mir
{
    using Microsoft.Xna.Framework;

    public interface ILight
    {
        Color LightColor { get; }

        float LightDistance { get; }

        Vector3 LightPosition { get; }
    }
}