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
            kernel.Load<Protogame3DIoCModule>();
            kernel.Load<ProtogameAssetIoCModule>();
            kernel.Load<ProtogameEventsIoCModule>();
            kernel.Load<ProtogameCollisionIoCModule>();
            kernel.Load<ProtogamePhysicsIoCModule>();
            kernel.Load<MirIocModule>();
            AssetManagerClient.AcceptArgumentsAndSetup<GameAssetManagerProvider>(kernel, args);

            using (var game = new MirGame(kernel))
            {
                game.Run();
            }
        }
    }
}

#endif
