namespace Mir
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class ShipEditorEntity : IEntity
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IGridRenderer m_GridRenderer;

        private readonly ICollision m_Collision;

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

        private RenderTarget2D m_PreviewTarget;

        private float m_PreviewRotation;

        private float m_CameraRotation;

        private float m_StartCameraRotation;

        public ShipEditorEntity(
            IAssetManagerProvider assetManagerProvider,
            I2DRenderUtilities twoRenderUtilities,
            IGridRenderer gridRenderer, 
            ICollision collision,
            Ship ship)
        {
            this.m_DefaultFont = assetManagerProvider.GetAssetManager().Get<FontAsset>("font.Default");
            this.m_2DRenderUtilities = twoRenderUtilities;
            this.m_GridRenderer = gridRenderer;
            this.m_Collision = collision;
            this.m_Ship = ship;

            this.m_PreviewRotation = 0;

            this.HorizontalRange = 30;
            this.VerticalRange = 30;
            this.VerticalSelection = 3;

            this.UpdateShipVisibilityCull();
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
            if (this.m_PreviewTarget == null)
            {
                this.m_PreviewTarget = new RenderTarget2D(
                    renderContext.GraphicsDevice, 
                    400,
                    240,
                    false,
                    gameContext.Graphics.PreferredBackBufferFormat,
                    gameContext.Graphics.PreferredDepthStencilFormat,
                    0,
                    RenderTargetUsage.PlatformContents);
            }

            if (renderContext.Is3DContext)
            {
                this.SetCamera(renderContext, this.m_CameraRotation);

                this.m_GridRenderer.Render(
                    renderContext,
                    this.GridX,
                    this.GridY,
                    this.GridZ,
                    this.HorizontalRange,
                    this.VerticalRange,
                    this.VerticalSelection,
                    new Color(31, 31, 31),
                    new Color(95, 31, 31));

                this.m_Ship.Render(gameContext, renderContext);

                renderContext.PushRenderTarget(this.m_PreviewTarget);

                renderContext.GraphicsDevice.Clear(Color.Black);

                this.SetCamera(renderContext, this.m_PreviewRotation, true);

                this.m_Ship.Render(gameContext, renderContext, true);

                renderContext.PopRenderTarget();

                this.SetCamera(renderContext, this.m_CameraRotation);
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

                this.m_2DRenderUtilities.RenderTexture(
                    renderContext,
                    new Vector2(
                        renderContext.GraphicsDevice.Viewport.Width - Math.Min(renderContext.GraphicsDevice.Viewport.Width / 4, 400),
                        renderContext.GraphicsDevice.Viewport.Height - Math.Min(renderContext.GraphicsDevice.Viewport.Height / 4, 240)),
                    new TextureAsset(this.m_PreviewTarget));
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
            else if (world.ActiveShipTool is PencilFillShipTool || world.ActiveShipTool is PencilClearShipTool ||
                world.ActiveShipTool is RectangleFillShipTool || world.ActiveShipTool is RectangleClearShipTool)
            {
                this.m_ToolIsActive = true;
            }
            else if (world.ActiveShipTool is RotateViewShipTool)
            {
                this.m_ToolIsActive = true;
                this.m_StartCameraRotation = this.m_CameraRotation;
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_PreviewRotation += (MathHelper.Pi * 2) / (360 * 5);

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

                this.UpdateShipVisibilityCull();
            }
            else if (world.ActiveShipTool is ZoomViewShipTool)
            {
                this.m_ZoomDistance = MathHelper.Clamp(
                    this.m_StartZoomDistance + ((mouse.Y - this.m_StartMouseY) / 10f),
                    -27f,
                    27f);
            }
            else if (world.ActiveShipTool is RotateViewShipTool)
            {
                this.m_CameraRotation = this.m_StartCameraRotation + ((mouse.X - this.m_StartMouseX) / 1000f);
            }
            else if (world.ActiveShipTool is ResizeSelectionShipTool)
            {
                this.VerticalSelection = (int)MathHelper.Clamp(
                    this.m_StartVerticalSelection - (int)((mouse.Y - this.m_StartMouseY) / 20f),
                    1,
                    10);

                this.UpdateShipVisibilityCull();
            }
            else if (world.ActiveShipTool is RectangleFillShipTool)
            {
                var point = this.GetMouseIntersectionPoint(gameContext);

                if (point != null)
                {
                    for (var x = -2; x <= 2; x++)
                    {
                        for (var z = -2; z <= 2; z++)
                        {
                            for (var i = 0; i < this.VerticalSelection; i++)
                            {
                                this.m_Ship.FillCell((int)point.Value.X + x, this.GridY + i, (int)point.Value.Y + z);
                            }
                        }
                    }
                }
            }
            else if (world.ActiveShipTool is RectangleClearShipTool)
            {
                var point = this.GetMouseIntersectionPoint(gameContext);

                if (point != null)
                {
                    for (var x = -2; x <= 2; x++)
                    {
                        for (var z = -2; z <= 2; z++)
                        {
                            for (var i = 0; i < this.VerticalSelection; i++)
                            {
                                this.m_Ship.ClearCell((int)point.Value.X + x, this.GridY + i, (int)point.Value.Y + z);
                            }
                        }
                    }
                }
            }
            else if (world.ActiveShipTool is PencilFillShipTool)
            {
                var point = this.GetMouseIntersectionPoint(gameContext);

                if (point != null)
                {
                    for (var i = 0; i < this.VerticalSelection; i++)
                    {
                        this.m_Ship.FillCell((int)point.Value.X, this.GridY + i, (int)point.Value.Y);
                    }
                }
            }
            else if (world.ActiveShipTool is PencilClearShipTool)
            {
                var point = this.GetMouseIntersectionPoint(gameContext);

                if (point != null)
                {
                    for (var i = 0; i < this.VerticalSelection; i++)
                    {
                        this.m_Ship.ClearCell((int)point.Value.X, this.GridY + i, (int)point.Value.Y);
                    }
                }
            }
        }

        private void UpdateShipVisibilityCull()
        {
            this.m_Ship.SetVerticalVisibilityCull(this.GridY + this.VerticalSelection - 1);
        }

        private void SetCamera(IRenderContext renderContext, float previewRotation = 0f, bool zoomIn = false)
        {
            var target = new Vector3(this.GridX, this.GridY, this.GridZ);

            var pos = new Vector3(
                (this.HorizontalRange / (zoomIn ? 2f : 1)) + this.m_ZoomDistance,
                (this.VerticalRange / (zoomIn ? 2f : 1)) + this.m_ZoomDistance,
                (this.HorizontalRange / (zoomIn ? 2f : 1)) + this.m_ZoomDistance);
            pos = Vector3.Transform(pos, Matrix.CreateRotationY(previewRotation));
            pos += target;

            renderContext.View =
                Matrix.CreateLookAt(
                    pos,
                    target, 
                    Vector3.Up);

            var viewport = renderContext.GraphicsDevice.Viewport;
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 
                viewport.Width / (float)viewport.Height, 
                1f, 
                5000f);
        }

        private Vector2? GetMouseIntersectionPoint(IGameContext gameContext)
        {
            var triangle1 = new[]
            {
                new Vector3(this.GridX - this.HorizontalRange, this.GridY, this.GridZ - this.HorizontalRange),
                new Vector3(this.GridX - this.HorizontalRange, this.GridY, this.GridZ + this.HorizontalRange),
                new Vector3(this.GridX + this.HorizontalRange, this.GridY, this.GridZ - this.HorizontalRange),
            };
            
            var triangle2 = new[]
            {
                new Vector3(this.GridX - this.HorizontalRange, this.GridY, this.GridZ + this.HorizontalRange),
                new Vector3(this.GridX + this.HorizontalRange, this.GridY, this.GridZ - this.HorizontalRange),
                new Vector3(this.GridX + this.HorizontalRange, this.GridY, this.GridZ + this.HorizontalRange),
            };

            float distance;
            var collision1 = this.m_Collision.CollidesWithTriangle(gameContext.MouseRay, triangle1, out distance, false);
            var collision2 = this.m_Collision.CollidesWithTriangle(gameContext.MouseRay, triangle2, out distance, false);

            var collision = collision1 ?? collision2;

            if (collision == null)
            {
                return null;
            }

            return new Vector2((int)collision.Value.X, (int)collision.Value.Z);
        }
    }
}