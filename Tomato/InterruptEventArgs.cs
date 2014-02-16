using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tomato.Hardware;

namespace Tomato
{
    public class InterruptEventArgs : EventArgs
    {
        public Device InterruptingDevice { get; set; }
        public ushort Message { get; set; }
    }
}
