namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class MainMenuWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly FontAsset m_RussianLargeFont;

        private readonly FontAsset m_RussianMediumFont;

        private int m_SelectedIndex;

        public MainMenuWorld(IAssetManagerProvider assetManagerProvider, I2DRenderUtilities twodRenderUtilities)
        {
            this.m_2DRenderUtilities = twodRenderUtilities;

            var assetManager = assetManagerProvider.GetAssetManager();

            this.m_RussianLargeFont = assetManager.Get<FontAsset>("font.RussianLarge");
            this.m_RussianMediumFont = assetManager.Get<FontAsset>("font.RussianMedium");

            this.Entities = new IEntity[0];

            this.Actions = new Dictionary<string, Action<IGameContext>>
            {
                { "-Hyperplayer", this.DoHyperplayer }, 
                { "-Multiplayer", this.DoMultiplayer }, 
                { "-Singleplayer", this.DoSingleplayer }, 
                { "Ship editor", this.DoShipEditor }, 
                { "Quit", this.DoQuit }
            };
        }

        public Dictionary<string, Action<IGameContext>> Actions { get; set; }

        public IList<IEntity> Entities { get; private set; }

        public void Dispose()
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                renderContext.GraphicsDevice.Clear(Color.Black);
            }
            else
            {
                this.m_2DRenderUtilities.RenderText(
                    renderContext, 
                    new Vector2(renderContext.GraphicsDevice.Viewport.Width - 40, 20), 
                    "Mir", 
                    this.m_RussianLargeFont, 
                    HorizontalAlignment.Right, 
                    textColor: Color.White);

                var mouse = Mouse.GetState();

                var maxSize =
                    this.Actions.Select(
                        x => this.m_2DRenderUtilities.MeasureText(renderContext, x.Key, this.m_RussianMediumFont))
                        .Max(x => x.X);

                var i = 0;
                this.m_SelectedIndex = -1;
                foreach (var kv in this.Actions)
                {
                    var x = 20;
                    var y = renderContext.GraphicsDevice.Viewport.Height - 24 - (this.Actions.Count * 34) + (i * 34);

                    var size = this.m_2DRenderUtilities.MeasureText(renderContext, kv.Key, this.m_RussianMediumFont);

                    var color = new Color(127, 127, 127);
                    if (new Rectangle(x, y + 12, (int)maxSize, (int)size.Y).Contains(mouse.Position))
                    {
                        color = Color.Orange;
                        this.m_SelectedIndex = i;
                    }

                    var text = kv.Key;
                    if (kv.Key.StartsWith("-"))
                    {
                        color = new Color(63, 63, 63);
                        text = kv.Key.Substring(1);
                    }

                    this.m_2DRenderUtilities.RenderText(
                        renderContext, 
                        new Vector2(x, y), 
                        text,  
                        this.m_RussianMediumFont, 
                        textColor: color);

                    i++;
                }
            }
        }

        public void Select(IGameContext gameContext)
        {
            if (this.m_SelectedIndex == -1)
            {
                return;
            }

            var i = 0;
            foreach (var kv in this.Actions)
            {
                if (i == this.m_SelectedIndex)
                {
                    kv.Value(gameContext);
                }

                i++;
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        private void DoHyperplayer(IGameContext obj)
        {
        }

        private void DoMultiplayer(IGameContext obj)
        {
        }

        private void DoQuit(IGameContext gameContext)
        {
            gameContext.Game.Exit();
        }

        private void DoShipEditor(IGameContext gameContext)
        {
            gameContext.SwitchWorld<RoomEditorWorld>();
        }

        private void DoSingleplayer(IGameContext obj)
        {
        }
    }
}