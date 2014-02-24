namespace Mir
{
    using Microsoft.Xna.Framework;

    public class EmptyLight : ILight
    {
        public Color LightColor
        {
            get
            {
                return Color.White;
            }
        }

        public float LightDistance
        {
            get
            {
                return 0;
            }
        }

        public Vector3 LightPosition
        {
            get
            {
                return Vector3.Zero;
            }
        }
    }
}