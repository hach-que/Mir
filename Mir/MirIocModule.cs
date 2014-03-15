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
            this.Bind<IEventBinder<IGameContext>>().To<ShipEditorStaticEventBinder>();
            this.Bind<IEventBinder<IGameContext>>().To<MainMenuStaticEventBinder>();

            this.Bind<IFactory>().ToFactory();
            this.Bind<IMeshCollider>().To<MeshCollider>();

            this.Bind<IRoomTool>().To<SizeRoomTool>();
            this.Bind<IRoomTool>().To<MoveRoomTool>();
            this.Bind<IRoomTool>().To<AngleRoomTool>();
            this.Bind<IRoomTool>().To<TextureRoomTool>();
            this.Bind<IRoomTool>().To<NewRoomTool>();
            this.Bind<IRoomTool>().To<NewRoomLightTool>();
            this.Bind<IRoomTool>().To<DeleteRoomTool>();
            this.Bind<IRoomTool>().To<DCPURoomTool>();
            this.Bind<IRoomTool>().To<ExitRoomTool>();

            this.Bind<IShipTool>().To<MoveViewShipTool>();
            this.Bind<IShipTool>().To<RotateViewShipTool>();
            this.Bind<IShipTool>().To<ZoomViewShipTool>();
            this.Bind<IShipTool>().To<ShiftSelectionShipTool>();
            this.Bind<IShipTool>().To<ResizeSelectionShipTool>();
            this.Bind<IShipTool>().To<RectangleFillShipTool>();
            this.Bind<IShipTool>().To<RectangleClearShipTool>();
            this.Bind<IShipTool>().To<PencilFillShipTool>();
            this.Bind<IShipTool>().To<PencilClearShipTool>();
            this.Bind<IShipTool>().To<CreateRoomShipTool>();
            this.Bind<IShipTool>().To<EnterRoomShipTool>();
            this.Bind<IShipTool>().To<ExitShipTool>();

            this.Bind<IShipStorage>().To<ShipStorage>();

            this.Bind<IGridRenderer>().To<DefaultGridRenderer>();
        }
    }
}