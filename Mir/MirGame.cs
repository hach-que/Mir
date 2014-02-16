namespace Mir
{
    using Ninject;

    using Protogame;

    public class MirGame : CoreGame<MirWorld, Default3DWorldManager>
    {
        public MirGame(StandardKernel kernel)
            : base(kernel)
        {
            this.IsMouseVisible = true;
        }
    }
}
