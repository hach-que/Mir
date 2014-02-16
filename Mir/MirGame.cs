namespace Mir
{
    using Ninject;

    using Protogame;

    public class MirGame : CoreGame<MirWorld, Default2DWorldManager>
    {
        public MirGame(StandardKernel kernel)
            : base(kernel)
        {
        }
    }
}
