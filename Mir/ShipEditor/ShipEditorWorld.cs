namespace Mir
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class ShipEditorWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IAssetManager m_AssetManager;

        private readonly FontAsset m_DefaultFont;

        private readonly Ship m_Ship;

        private readonly ShipEditorEntity m_ShipEditorEntity;

        private readonly IShipTool[] m_ShipTools;

        private int m_ActiveTool;

        public ShipEditorWorld(
            IFactory factory, 
            I2DRenderUtilities twoDRenderUtilities, 
            IAssetManagerProvider assetManagerProvider, 
            IShipTool[] shipTools)
        {
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_ShipTools = shipTools;
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");

            this.m_Ship = factory.CreateShip();

            this.m_ShipEditorEntity = factory.CreateShipEditorEntity(this.m_Ship);

            this.Entities.Add(this.m_ShipEditorEntity);
        }

        public IShipTool ActiveShipTool
        {
            get
            {
                return this.m_ShipTools[this.m_ActiveTool];
            }
        }

        public IList<IEntity> Entities { get; private set; }

        public void ActivateAlternate()
        {
            this.m_ShipEditorEntity.UseAlternative = true;
        }

        public void DeactivateAlternate()
        {
            this.m_ShipEditorEntity.UseAlternative = false;
        }

        public void Dispose()
        {
        }

        public void ReleaseTool(IGameContext gameContext)
        {
            this.m_ShipEditorEntity.ReleaseCurrentSelection(gameContext);
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                renderContext.PopEffect();

                return;
            }

            for (var i = 0; i < this.m_ShipTools.Length; i++)
            {
                var tool = this.m_ShipTools[i];

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
                        this.m_ShipTools[this.m_ActiveTool].Name, 
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
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var mouse = Mouse.GetState();
            var value = mouse.ScrollWheelValue;
            while (value < 0)
            {
                value += 120 * this.m_ShipTools.Length;
            }

            this.m_ActiveTool = this.m_ShipTools.Length - ((value / 120) % this.m_ShipTools.Length);
            if (this.m_ActiveTool == this.m_ShipTools.Length)
            {
                this.m_ActiveTool = 0;
            }
        }

        public void UseTool(IGameContext gameContext, bool secondaryAlt)
        {
            this.m_ShipEditorEntity.SelectCurrentHover(gameContext, secondaryAlt);
        }
    }
}