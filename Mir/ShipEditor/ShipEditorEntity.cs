namespace Mir
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class ShipEditorEntity : IEntity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IGridRenderer m_GridRenderer;

        private readonly Ship m_Ship;

        private readonly FontAsset m_DefaultFont;

        private int m_StartMouseX;

        private int m_StartMouseY;

        private int m_StartGridX;

        private int m_StartGridY;

        private int m_StartGridZ;

        private bool m_ToolIsActive;

        private float m_ZoomDistance;

        private float m_StartZoomDistance;

        private float m_StartVerticalSelection;

        public ShipEditorEntity(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities twoRenderUtilities,
            IGridRenderer gridRenderer, 
            Ship ship)
        {
            this.m_DefaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            this.m_2DRenderUtilities = twoRenderUtilities;
            this.m_GridRenderer = gridRenderer;
            this.m_Ship = ship;

            this.HorizontalRange = 30;
            this.VerticalRange = 30;
            this.VerticalSelection = 1;
        }

        public int GridX { get; set; }

        public int GridY { get; set; }

        public int GridZ { get; set; }

        public int HorizontalRange { get; set; }

        public bool UseAlternative { get; set; }

        public int VerticalRange { get; set; }

        public int VerticalSelection { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void ReleaseCurrentSelection()
        {
            this.m_ToolIsActive = false;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                this.SetCamera(renderContext);

                this.m_GridRenderer.Render(
                    renderContext,
                    this.GridX,
                    this.GridY,
                    this.GridZ,
                    this.HorizontalRange,
                    this.VerticalRange,
                    this.VerticalSelection,
                    new Color(63, 63, 63),
                    new Color(127, 63, 63));
            }
            else
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext,
                    new Vector2(
                        renderContext.GraphicsDevice.Viewport.Width - 10,
                        10),
                        "Focus: (" + this.GridX + ", " + this.GridY + ", " + this.GridZ + ")",
                        this.m_DefaultFont,
                        horizontalAlignment: HorizontalAlignment.Right);
            }
        }

        public void SelectCurrentHover(IGameContext gameContext, bool secondaryAlt)
        {
            var world = (ShipEditorWorld)gameContext.World;

            var mouse = Mouse.GetState();
            this.m_StartMouseX = mouse.X;
            this.m_StartMouseY = mouse.Y;

            if (world.ActiveShipTool is EnterRoomShipTool)
            {
                gameContext.SwitchWorld<RoomEditorWorld>();
            }
            else if (world.ActiveShipTool is ExitShipTool)
            {
                gameContext.SwitchWorld<MainMenuWorld>();
            }
            else if (world.ActiveShipTool is MoveViewShipTool || world.ActiveShipTool is ShiftSelectionShipTool)
            {
                this.m_StartGridX = this.GridX;
                this.m_StartGridY = this.GridY;
                this.m_StartGridZ = this.GridZ;
                this.m_ToolIsActive = true;
            }
            else if (world.ActiveShipTool is ZoomViewShipTool)
            {
                this.m_ToolIsActive = true;
                this.m_StartZoomDistance = this.m_ZoomDistance;
            }
            else if (world.ActiveShipTool is ResizeSelectionShipTool)
            {
                this.m_ToolIsActive = true;
                this.m_StartVerticalSelection = this.VerticalSelection;
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (!this.m_ToolIsActive)
            {
                return;
            }

            var world = (ShipEditorWorld)gameContext.World;

            var mouse = Mouse.GetState();

            if (world.ActiveShipTool is MoveViewShipTool)
            {
                var diffVector = new Vector3(mouse.X - this.m_StartMouseX, 0, mouse.Y - this.m_StartMouseY);
                diffVector = Vector3.Transform(diffVector, Matrix.CreateRotationY(MathHelper.ToRadians(45)));

                this.GridX = this.m_StartGridX - (int)(diffVector.X / 10);
                this.GridZ = this.m_StartGridZ - (int)(diffVector.Z / 10);
            }
            else if (world.ActiveShipTool is ShiftSelectionShipTool)
            {
                this.GridY = this.m_StartGridY - (int)((mouse.Y - this.m_StartMouseY) / 20f);
            }
            else if (world.ActiveShipTool is ZoomViewShipTool)
            {
                this.m_ZoomDistance = MathHelper.Clamp(
                    this.m_StartZoomDistance + ((mouse.Y - this.m_StartMouseY) / 10f),
                    -27f,
                    27f);
            }
            else if (world.ActiveShipTool is ResizeSelectionShipTool)
            {
                this.VerticalSelection = (int)MathHelper.Clamp(
                    this.m_StartVerticalSelection - (int)((mouse.Y - this.m_StartMouseY) / 20f),
                    1,
                    10);
            }
        }

        private void SetCamera(IRenderContext renderContext)
        {
            renderContext.View =
                Matrix.CreateLookAt(
                    new Vector3(
                        this.GridX + this.HorizontalRange + this.m_ZoomDistance,
                        this.GridY + this.VerticalRange + this.m_ZoomDistance,
                        this.GridZ + this.HorizontalRange + this.m_ZoomDistance), 
                    new Vector3(
                        this.GridX,
                        this.GridY,
                        this.GridZ), 
                    Vector3.Up);

            var viewport = renderContext.GraphicsDevice.Viewport;
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 
                viewport.Width / (float)viewport.Height, 
                1f, 
                5000f);
        }
    }
}