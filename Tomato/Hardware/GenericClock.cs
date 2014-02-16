using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace Tomato.Hardware
{
    public class GenericClock : Device
    {
        [Category("Device Status")]
        [TypeConverter(typeof(HexTypeEditor))]
        public ushort InterruptMessage { get; set; }
        [Category("Device Status")]
        public ushort Frequency { get; set; }
        [Category("Device Status")]
        public ushort ElapsedTicks { get; set; }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint DeviceID
        {
            get { return 0x12d0b402; }
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint ManufacturerID
        {
            get { return 0x0; }
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override ushort Version
        {
            get { return 1; }
        }

        [Browsable(false)]
        public override string FriendlyName
        {
            get { return "Generic Clock (compatible)"; }
        }

        private int ElapsedHardwareTicks;

        public override int HandleInterrupt()
        {
            switch (AttachedCPU.A)
            {
                case 0:
                    Frequency = AttachedCPU.B;
                    ElapsedTicks = 0;
                    break;
                case 1:
                    AttachedCPU.C = ElapsedTicks;
                    break;
                case 2:
                    InterruptMessage = AttachedCPU.B;
                    break;
            }
            return 0;
        }

        public override void Tick()
        {
            ElapsedHardwareTicks++;
            if (ElapsedHardwareTicks >= Frequency)
            {
                ElapsedHardwareTicks = 0;
                ElapsedTicks++;
                if (InterruptMessage != 0)
                    AttachedCPU.FireInterrupt(InterruptMessage);
            }
            base.Tick();
        }

        public override void Reset()
        {
            ElapsedTicks = InterruptMessage = Frequency = 0;
        }
    }
}
