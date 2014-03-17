namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;
    using Tomato.Hardware;

    public class LEM1802RoomObject : RoomObject, IConnectable
    {
        private LEM1802 m_LEM1802;

        private RenderTarget2D m_RenderTarget;

        private TextureAsset m_ShipTextureAsset;

        public LEM1802RoomObject(IAssetManagerProvider assetManagerProvider, bool temporary)
        {
            this.Connections = new List<IConnectable>();
            this.m_ShipTextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");
            this.ForceFullTexture = true;
        }

        public List<IConnectable> Connections { get; set; }

        public Device TomatoDevice
        {
            get
            {
                return this.m_LEM1802;
            }
        }

        public override string Type
        {
            get
            {
                return "lem1802";
            }
        }

        public void ConnectionsUpdated()
        {
        }

        public void PrepareDevice()
        {
            this.m_LEM1802 = new LEM1802();
        }

        public override void Reinitalize()
        {
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            // Render the LEM1802 to a texture
            if (this.m_LEM1802 != null)
            {
                this.RenderDCPU(renderContext);
            }
            else
            {
                if (this.m_RenderTarget == null)
                {
                    this.m_RenderTarget = new RenderTarget2D(renderContext.GraphicsDevice, LEM1802.Width, LEM1802.Height);
                }

                renderContext.PushRenderTarget(this.m_RenderTarget);
                renderContext.GraphicsDevice.Clear(Color.Black);
                renderContext.PopRenderTarget();
            }

            if (this.m_PendRecalculation)
            {
                this.RecalculateObject(gameContext, renderContext);
            }

            renderContext.GraphicsDevice.Indices = this.m_IndexBuffer;
            renderContext.GraphicsDevice.SetVertexBuffer(this.m_VertexBuffer);

            renderContext.SetActiveTexture(this.m_RenderTarget);

            renderContext.GraphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                this.m_VertexBuffer.VertexCount,
                0,
                this.m_IndexBuffer.IndexCount / 3);

            renderContext.SetActiveTexture(this.m_ShipTextureAsset.Texture);

            this.m_TouchAnimIndex++;
            if (this.m_TouchAnimIndex >= this.m_TouchAnim.Length * 3)
            {
                this.m_TouchAnimIndex = 0;
            }
        }

        private void RenderDCPU(IRenderContext renderContext)
        {
            if (this.m_RenderTarget == null)
            {
                this.m_RenderTarget = new RenderTarget2D(renderContext.GraphicsDevice, LEM1802.Width, LEM1802.Height);
            }

            // Copied from LEM1802.
            if (this.m_LEM1802.ScreenMap == 0)
            {
                return;
            }

            var pixels = new Color[LEM1802.Width * LEM1802.Height];

            ushort address = 0;
            for (var y = 0; y < 12; y++)
            {
                for (var x = 0; x < 32; x++)
                {
                    var value = this.m_LEM1802.AttachedCPU.Memory[this.m_LEM1802.ScreenMap + address];
                    uint fontValue;
                    if (this.m_LEM1802.FontMap == 0)
                    {
                        fontValue =
                            (uint)
                            ((LEM1802.DefaultFont[(value & 0x7F) * 2] << 16)
                             | LEM1802.DefaultFont[(value & 0x7F) * 2 + 1]);
                    }
                    else
                    {
                        fontValue =
                            (uint)
                            ((this.m_LEM1802.AttachedCPU.Memory[this.m_LEM1802.FontMap + ((value & 0x7F) * 2)] << 16)
                             | this.m_LEM1802.AttachedCPU.Memory[this.m_LEM1802.FontMap + ((value & 0x7F) * 2) + 1]);
                    }

                    fontValue = BitConverter.ToUInt32(BitConverter.GetBytes(fontValue).Reverse().ToArray(), 0);

                    var backgroundc = this.m_LEM1802.GetPaletteColor((byte)((value & 0xF00) >> 8));
                    var foregroundc = this.m_LEM1802.GetPaletteColor((byte)((value & 0xF000) >> 12));
                    var background = new Color(backgroundc.R, backgroundc.G, backgroundc.B, backgroundc.A);
                    var foreground = new Color(foregroundc.R, foregroundc.G, foregroundc.B, foregroundc.A);
                    for (var i = 0; i < sizeof(uint) * 8; i++)
                    {
                        var px = i / 8 + (x * LEM1802.CharWidth);
                        var py = i % 8 + (y * LEM1802.CharHeight);
                        if ((fontValue & 1) == 0 || (((value & 0x80) == 0x80) && !this.m_LEM1802.BlinkOn))
                        {
                            pixels[px + py * LEM1802.Width] = background;
                        }
                        else
                        {
                            pixels[px + py * LEM1802.Width] = foreground;
                        }

                        fontValue >>= 1;
                    }

                    address++;
                }
            }

            this.m_RenderTarget.SetData(pixels);
        }
    }
}