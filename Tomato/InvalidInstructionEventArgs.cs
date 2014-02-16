using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomato
{
    public class InvalidInstructionEventArgs : EventArgs
    {
        public ushort Instruction { get; set; }
        public ushort Address { get; set; }
        public bool ContinueExecution { get; set; }

        public InvalidInstructionEventArgs(ushort instruction, ushort address)
        {
            Instruction = instruction;
            Address = address;
            ContinueExecution = true;
        }
    }
}
