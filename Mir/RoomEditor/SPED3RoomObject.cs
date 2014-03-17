namespace Mir
{
    using System.Collections.Generic;
    using Protogame;
    using Tomato.Hardware;

    public class SPED3RoomObject : RoomObject, IConnectable
    {
        private SPED3 m_SPED3;

        public SPED3RoomObject(bool temporary)
        {
            this.Connections = new List<IConnectable>();
        }

        public List<IConnectable> Connections { get; set; }

        public Device TomatoDevice
        {
            get
            {
                return this.m_SPED3;
            }
        }

        public override string Type
        {
            get
            {
                return "sped3";
            }
        }

        public void ConnectionsUpdated()
        {
        }

        public void PrepareDevice()
        {
            this.m_SPED3 = new SPED3();
        }

        public override void Reinitalize()
        {
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            // TODO: Custom render
            base.Render(gameContext, renderContext);
        }
    }
}