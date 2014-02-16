#if PLATFORM_WINDOWS || PLATFORM_MACOS || PLATFORM_LINUX

namespace Mir
{
    using Ninject;

    using Protogame;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load<Protogame2DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, args);

            using (var game = new MirGame(kernel))
            {
                game.Run();
            }
        }
    }
}

#endif
