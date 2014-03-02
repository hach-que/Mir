namespace Mir
{
    using Ninject.Extensions.Factory;
    using Ninject.Modules;
    using Protogame;

    public class MirIocModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEventBinder<IGameContext>>().To<RoomEditorStaticEventBinder>();
            this.Bind<IEventBinder<IGameContext>>().To<MainMenuStaticEventBinder>();

            this.Bind<IFactory>().ToFactory();
            this.Bind<IMeshCollider>().To<MeshCollider>();

            this.Bind<ITool>().To<SizeTool>();
            this.Bind<ITool>().To<MoveTool>();
            this.Bind<ITool>().To<AngleTool>();
            this.Bind<ITool>().To<TextureTool>();
            this.Bind<ITool>().To<NewTool>();
            this.Bind<ITool>().To<NewLightTool>();
            this.Bind<ITool>().To<DeleteTool>();
            this.Bind<ITool>().To<DCPUTool>();
        }
    }
}