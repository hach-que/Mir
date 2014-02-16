using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Tomato.Hardware
{
    public abstract class Device
    {
        public abstract uint DeviceID { get; }
        public abstract uint ManufacturerID { get; }
        public abstract ushort Version { get; }
        public abstract string FriendlyName { get; }
        public DCPU AttachedCPU;
        public event EventHandler InterruptFired;

        [Browsable(false)]
        public virtual bool SelectedByDefault
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the number of cycles required to perform the interrupt
        /// </summary>
        /// <returns></returns>
        public abstract int HandleInterrupt();
        public abstract void Reset();

        public int DoInterrupt()
        {
            if (InterruptFired != null)
                InterruptFired(this, null);
            return HandleInterrupt();
        }

        /// <summary>
        /// Called every 1667 cycles (roughly 60 Hz)
        /// </summary>
        public virtual void Tick()
        {
        }

        public bool BreakOnInterrupt { get; set; }

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}
