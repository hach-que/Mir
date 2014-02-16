using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Tomato.Hardware
{
    public class SPC2000 : Device
    {
        [Category("Device Status")]
        public uint UnitToSkip { get; set; }
        [Category("Device Status")]
        public ushort SkipUnit { get; set; }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint DeviceID
        {
            get { return 0x40e41d9d; }
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint ManufacturerID
        {
            get { return 0x1c6c8b36; }
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override ushort Version
        {
            get { return 0x005e; }
        }

        [Browsable(false)]
        public override string FriendlyName
        {
            get { return "Suspension Chamber 2000"; }
        }

        [Browsable(false)]
        public override bool SelectedByDefault
        {
            get
            {
                return false;
            }
        }

        public override int HandleInterrupt()
        {
            switch (AttachedCPU.A)
            {
                case 0:
                    GetStatus();
                    break;
                case 1:
                    UnitToSkip = (uint)(AttachedCPU.Memory[AttachedCPU.B] << 16);
                    UnitToSkip |= AttachedCPU.Memory[AttachedCPU.B + 1];
                    break;
                case 2:
                    GetStatus();
                    if (AttachedCPU.C == 1)
                    {
                        // Trigger device
                        // Not sure what to do here.
                    }
                    break;
                case 3:
                    SkipUnit = AttachedCPU.B;
                    break;
            }
            return 0;
        }

        private void GetStatus()
        {
            AttachedCPU.C = 0;
            AttachedCPU.B = 6; // Mechanical error
        }

        public override void Reset()
        {
            SkipUnit = 0;
            UnitToSkip = 0;
        }
    }
}
