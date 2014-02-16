using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomato
{
    public class Breakpoint
    {
        public ushort Address { get; set; }
        public Dictionary<string, object> Tags { get; set; }
    }
}
