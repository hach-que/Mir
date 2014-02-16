using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomato
{
    /// <summary>
    /// Represents a line of disassembled code.
    /// </summary>
    public class CodeEntry
    {
        public CodeEntry()
        {
            IsLabel = false;
        }

        public string Code { get; set; }
        public string OpcodeText { get; set; }
        public string ValueAText { get; set; }
        public string ValueBText { get; set; }
        public byte Opcode { get; set; }
        public byte ValueA { get; set; }
        public byte ValueB { get; set; }
        public ushort Address { get; set; }
        public bool IsLabel { get; set; }
    }

    public enum CodeEntryType
    {
        Data,
        Code,
    }
}
