namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Protogame;
    using Tomato;
    using Tomato.Hardware;

    public class DCPURoomObject : RoomObject, IConnectable
    {
        private DCPU m_DCPU;

        private bool m_Spawned;

        private int m_TextureCounter;

        public DCPURoomObject(bool temporary)
        {
            this.m_Spawned = temporary;
            this.Connections = new List<IConnectable>();
        }

        public List<IConnectable> Connections { get; set; }

        public Device TomatoDevice
        {
            get
            {
                return null;
            }
        }

        public override string Type
        {
            get
            {
                return "dcpu";
            }
        }

        public void ConnectionsUpdated()
        {
            // This will cause DCPU reinit.
            this.m_Spawned = false;
        }

        public override void Reinitalize()
        {
            this.m_Spawned = false;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (this.m_DCPU != null)
            {
                this.m_DCPU.Execute((int)(100000 * gameContext.GameTime.ElapsedGameTime.TotalSeconds));
            }

            this.m_TextureCounter++;
            if (this.m_TextureCounter >= 5 * 30)
            {
                this.m_TextureCounter = 0;
            }

            var textureCounter = this.m_TextureCounter / 30;

            this.AboveTextureIndex = 12 + textureCounter;
            this.BelowTextureIndex = 12 + textureCounter;
            this.LeftTextureIndex = 12 + textureCounter;
            this.RightTextureIndex = 12 + textureCounter;
            this.FrontTextureIndex = 12 + textureCounter;
            this.BackTextureIndex = 12 + textureCounter;

            base.Render(gameContext, renderContext);

            if (this.m_Spawned)
            {
                return;
            }

            this.m_DCPU = new DCPU();

            foreach (var connection in this.Connections)
            {
                connection.PrepareDevice();
                this.m_DCPU.ConnectDevice(connection.TomatoDevice);
            }

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

            this.m_Spawned = true;
        }

        public void PrepareDevice()
        {
        }
    }
}