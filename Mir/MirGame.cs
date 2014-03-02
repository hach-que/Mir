namespace Mir
{
    using Microsoft.Xna.Framework.Graphics;
    using Ninject;
    using Protogame;

    public class MirGame : CoreGame<RoomEditorWorld, Default3DWorldManager>
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
            this.Window.AllowUserResizing = true;

            this.GraphicsDevice.SamplerStates[0].Filter = TextureFilter.Point;
        }
    }
}