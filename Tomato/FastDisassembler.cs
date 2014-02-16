using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Tomato
{
    public class FastDisassembler
    {
        #region Constructor and runtime values

        public bool AllowSigned = true;

        private ushort PC;
        private ushort[] Data;
        private Dictionary<ushort, string> KnownLabels;

        public FastDisassembler(Dictionary<ushort, string> KnownLabels)
        {
            this.KnownLabels = new Dictionary<ushort,string>(KnownLabels);
        }

        #endregion

        /// <summary>
        /// Disassembles a small snippet of data without finding labels.
        /// </summary>
        /// <param name="Data"></param>
        public List<CodeEntry> FastDisassemble(ref ushort[] Data, ushort PCStart, ushort StopAt)
        {
            List<CodeEntry> output = new List<CodeEntry>();
            this.Data = Data;
            this.PC = PCStart;

            while (PC != StopAt && PC - 1 != StopAt && PC - 2 != StopAt)
            {
                if (KnownLabels.ContainsKey(PC))
                {
                    output.Add(new CodeEntry()
                    {
                        Code = KnownLabels[PC] + ":",
                        IsLabel = true,
                        Address = PC
                    });
                    KnownLabels.Remove(PC);
                    continue;
                }
                ushort instruction = Data[PC++];
                byte opcode = (byte)(instruction & 0x1F);
                byte valueB = (byte)((instruction & 0x3E0) >> 5);
                byte valueA = (byte)((instruction & 0xFC00) >> 10);

                CodeEntry entry = new CodeEntry();
                entry.Opcode = opcode;
                entry.ValueA = valueA;
                entry.ValueB = valueB;
                entry.Address = (ushort)(PC - 1);
                if (entry.Opcode != 0)
                    entry.Code = GetOp(opcode);
                else
                    entry.Code = GetNonbasicOp(valueB);
                entry.OpcodeText = entry.Code;
                if (entry.Code == null)
                {
                    CodeEntry datEntry = new CodeEntry();
                    datEntry.Code = "DAT 0x" + instruction.ToString("x");
                    datEntry.Address = (ushort)(PC - 1);
                    output.Add(datEntry);
                }
                else
                {
                    if (entry.Opcode != 0)
                    {
                        string valueAdis = GetValue(valueA, false);
                        string valueBdis = GetValue(valueB, true);
                        entry.ValueAText = valueAdis;
                        entry.ValueBText = valueBdis;
                        entry.Code += " " + valueBdis + ", " + valueAdis;
                        output.Add(entry);
                    }
                    else
                    {
                        string valueAdis = GetValue(valueA, false);
                        entry.Code += " " + valueAdis;
                        entry.ValueAText = valueAdis;
                        output.Add(entry);
                    }
                }
            }

            return output;
        }

        private string GetNonbasicOp(byte opcode)
        {
            switch (opcode)
            {
                case 0x01: // JSR a
                    return "JSR";
                case 0x08: // INT a
                    return "INT";
                case 0x09: // IAG a
                    return "IAG";
                case 0x0A: // IAS a
                    return "IAS";
                case 0x0B: // RFI a
                    return "RFI";
                case 0x0C: // IAQ a
                    return "IAQ";
                case 0x10: // HWN a
                    return "HWN";
                case 0x11: // HWQ a
                    return "HWQ";
                case 0x12: // HWI a
                    return "HWI";
                default:
                    return null;
            }
        }

        private string GetOp(byte opcode)
        {
            switch (opcode)
            {
                case 0x01:
                    return "SET";
                case 0x02: // ADD b, a
                    return "ADD";
                case 0x03: // SUB b, a
                    return "SUB";
                case 0x04: // MUL b, a
                    return "MUL";
                case 0x05: // MLI b, a
                    return "MLI";
                case 0x06: // DIV b, a
                    return "DIV";
                case 0x07: // DVI b, a
                    return "DVI";
                case 0x08: // MOD b, a
                    return "MOD";
                case 0x09: // MDI b, a
                    return "MDI";
                case 0x0A: // AND b, a
                    return "AND";
                case 0x0B: // BOR b, a
                    return "BOR";
                case 0x0C: // XOR b, a
                    return "XOR";
                case 0x0D: // SHR b, a
                    return "SHR";
                case 0x0E: // ASR b, a
                    return "ASR";
                case 0x0F: // SHL b, a
                    return "SHL";
                case 0x10: // IFB b, a
                    return "IFB";
                case 0x11: // IFC b, a
                    return "IFC";
                case 0x12: // IFE b, a
                    return "IFE";
                case 0x13: // IFN b, a
                    return "IFN";
                case 0x14: // IFG b, a
                    return "IFG";
                case 0x15: // IFA b, a
                    return "IFA";
                case 0x16: // IFL b, a
                    return "IFL";
                case 0x17: // IFU b, a
                    return "IFU";
                case 0x1A: // ADX b, a
                    return "ADX";
                case 0x1B: // SBX b, a
                    return "SBX";
                case 0x1E: // STI b, a
                    return "STI";
                case 0x1F: // STD b, a
                    return "STD";
                default:
                    return null;
            }
        }

        private string GetValue(byte target, bool IsBContext)
        {
            if (target <= 0xF)
            {
                return new string[]
                {
                    "A", "B", "C", "X", "Y", "Z", "I", "J", 
                    "[A]", "[B]", "[C]", "[X]", "[Y]", "[Z]", "[I]", "[J]"
                }[target];
            }
            else if (target <= 0x17)
                return "[" + GetValue((byte)(target - 0x10), IsBContext) + " + 0x" + Data[PC++].ToString("x") + "]";
            else if (target == 0x18)
            {
                if (IsBContext)
                    return "PUSH";
                else
                    return "POP";
            }
            else if (target == 0x19)
                return "[SP]";
            else if (target == 0x1a)
                return "[SP + 0x" + Data[PC++].ToString("x") + "]";
            else if (target <= 0x1d)
            {
                return new string[]
                {
                    "SP",
                    "PC",
                    "EX"
                }[target - 0x1b];
            }
            else if (target == 0x1e)
                return "[0x" + Data[PC++].ToString("x") + "]";
            else if (target == 0x1f)
                return "0x" + Data[PC++].ToString("x");

            if (AllowSigned)
                return ((short)(target - 0x21)).ToString();
            else
                return "0x" + ((ushort)(target - 0x21)).ToString("x");
        }
    }
}
