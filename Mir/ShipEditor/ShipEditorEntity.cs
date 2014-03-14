namespace Mir
{
    using Protogame;

    public class ShipEditorEntity : IEntity
    {
        private readonly Ship m_Ship;

        public ShipEditorEntity(Ship ship)
        {
            this.m_Ship = ship;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public bool UseAlternative { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        public void ReleaseCurrentSelection()
        {
        }

        public void SelectCurrentHover(IGameContext gameContext, bool secondaryAlt)
        {
            var world = (ShipEditorWorld)gameContext.World;

            if (world.ActiveShipTool is EnterRoomShipTool)
            {
                gameContext.SwitchWorld<RoomEditorWorld>();
            }
            else if (world.ActiveShipTool is ExitShipTool)
            {
                gameContext.SwitchWorld<MainMenuWorld>();
            }
        }
    }
}