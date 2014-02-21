namespace Mir
{
    using Ninject;

    using Protogame;

    public class MirGame : CoreGame<MirWorld, DeferredLighting3DWorldManager>
    {
        public MirGame(StandardKernel kernel)
            : base(kernel)
        {
            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.Window.Title = "Mir";

            this.GraphicsDevice.SamplerStates[0].Filter = Microsoft.Xna.Framework.Graphics.TextureFilter.Point;
        }
    }
}
