namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class RoomEditorWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IAssetManager m_AssetManager;

        private readonly ITool[] m_Tools;

        private readonly RoomEditorEntity m_RoomEditorEntity;

        private readonly FontAsset m_DefaultFont;

        private int m_ActiveTool;

        private float m_NameFade;

        public RoomEditorWorld(
            IFactory factory, 
            I2DRenderUtilities twoDRenderUtilities, 
            IAssetManagerProvider assetManagerProvider, 
            ITool[] tools)
        {
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_Tools = tools;
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");

            var ship = factory.CreateShipEntity();
            var player = factory.CreatePlayerEntity();
            player.ParentArea = ship;

            var dcpu = factory.CreateDCPUEntity();
            dcpu.X = 20;
            dcpu.Y = 6;
            dcpu.Z = 20;

            this.m_RoomEditorEntity = factory.CreateRoomEditorEntity(factory.CreateRoom());

            this.Entities.Add(ship);
            this.Entities.Add(player);
            this.Entities.Add(dcpu);

            this.Entities.Add(this.m_RoomEditorEntity);
        }

        public IList<IEntity> Entities { get; private set; }

        public ITool ActiveTool
        {
            get
            {
                return this.m_Tools[this.m_ActiveTool];
            }
        }

        public void Dispose()
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                return;
            }

            for (var i = 0; i < this.m_Tools.Length; i++)
            {
                var tool = this.m_Tools[i];

                var texture = this.m_AssetManager.Get<TextureAsset>(tool.TextureName);

                this.m_2DRenderUtilities.RenderRectangle(
                    renderContext, 
                    new Rectangle(16, 16 + (i * 32), 24, 24), 
                    this.m_ActiveTool == i ? Color.Orange : Color.Black, 
                    true);

                if (this.m_ActiveTool == i)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(16 + 32, 16 + (i * 32) + 5),
                        this.m_Tools[this.m_ActiveTool].Name,
                        this.m_DefaultFont);
                }

                this.m_2DRenderUtilities.RenderTexture(renderContext, new Vector2(20, 20 + (i * 32)), texture);
            }
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            renderContext.GraphicsDevice.Clear(Color.Black);

            var player = this.Entities.OfType<PlayerEntity>().First();

            player.SetCamera(renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            // TODO: Do this through the event system
            var mouse = Mouse.GetState();
            var value = mouse.ScrollWheelValue;
            while (value < 0)
            {
                value += 120 * this.m_Tools.Length;
            }

            this.m_ActiveTool = this.m_Tools.Length - ((value / 120) % this.m_Tools.Length);
            if (this.m_ActiveTool == this.m_Tools.Length)
            {
                this.m_ActiveTool = 0;
            }
        }

        public void UseTool()
        {
            this.m_RoomEditorEntity.SelectCurrentHover();
        }

        public void ReleaseTool()
        {
            this.m_RoomEditorEntity.ReleaseCurrentSelection();
        }
    }
}