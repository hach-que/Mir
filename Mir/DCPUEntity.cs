using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mir
{
    using System.Reflection;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Protogame;
    using Tomato;
    using Tomato.Hardware;

    public class DCPUEntity : IEntity
    {
        private readonly I3DRenderUtilities m_3DRenderUtilities;

        private readonly DCPU m_DCPU;

        private readonly GenericKeyboard m_GenericKeyboard;

        private readonly LEM1802 m_LEM1802;

        private RenderTarget2D m_DCPURenderTarget;

        public DCPUEntity(I3DRenderUtilities threedRenderUtilities)
        {
            this.m_3DRenderUtilities = threedRenderUtilities;

            this.m_LEM1802 = new LEM1802();
            this.m_GenericKeyboard = new GenericKeyboard();
            this.m_DCPU = new DCPU();
            this.m_DCPU.ConnectDevice(this.m_LEM1802);
            this.m_DCPU.ConnectDevice(this.m_GenericKeyboard);

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mir.test.bin");
            var program = new ushort[stream.Length / 2];
            for (var i = 0; i < program.Length; i++)
            {
                var left = (byte)stream.ReadByte();
                var right = (byte)stream.ReadByte();
                var value = (ushort)(right | (left << 8));
                program[i] = value;
            }

            this.m_DCPU.FlashMemory(program);
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public bool AcceptingInput { get; set; }

        public Matrix Orientation { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            this.RenderDCPU(renderContext);

            this.m_3DRenderUtilities.RenderCube(
                renderContext,
                Matrix.CreateScale(2, 2, 2) * this.Orientation * Matrix.CreateTranslation(this.X, this.Y, this.Z),
                new TextureAsset(this.m_DCPURenderTarget),
                Vector2.Zero,
                Vector2.One);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_DCPU.Execute((int)(100000 * gameContext.GameTime.ElapsedGameTime.TotalSeconds));

            var world = (RoomEditorWorld)gameContext.World;

            if (!(world.ActiveTool is DCPUTool))
            {
                return;
            }

            var keyboard = Keyboard.GetState();
            var values = Enum.GetValues(typeof(Keys));
            var winNames = Enum.GetNames(typeof(System.Windows.Forms.Keys));
            foreach (var id in values)
            {
                var name = Enum.GetName(typeof(Keys), id);
                if (!winNames.Contains(name))
                {
                    if (name == "LeftShift" || name == "RightShift")
                    {
                        name = "Shift";
                    }
                    else if (name == "LeftControl" || name == "RightControl")
                    {
                        name = "Control";
                    }
                    else
                    {
                        continue;
                    }
                }

                if (keyboard.IsKeyChanged(this, (Keys)id) == KeyState.Down)
                {
                    this.m_GenericKeyboard.KeyDown(
                        (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), name));
                }

                if (keyboard.IsKeyUp((Keys)id))
                {
                    this.m_GenericKeyboard.KeyUp(
                        (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), name));
                }
            }
        }

        private void RenderDCPU(IRenderContext renderContext)
        {
            if (this.m_DCPURenderTarget == null)
            {
                this.m_DCPURenderTarget = new RenderTarget2D(
                    renderContext.GraphicsDevice,
                    LEM1802.Width,
                    LEM1802.Height);
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

            this.m_DCPURenderTarget.SetData(pixels);
        }
    }
}
