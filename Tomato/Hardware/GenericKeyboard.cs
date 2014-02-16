using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Tomato.Hardware
{
    public class GenericKeyboard : Device
    {
        [Category("Device Status")]
        public ushort InterruptMessage { get; set; }
        [Browsable(false)]
        public Queue<ushort> Buffer { get; set; }
        [Browsable(false)]
        public List<ushort> PressedKeys { get; set; }
        private Dictionary<Keys, string> ExtraMappings { get; set; }

        public GenericKeyboard()
        {
            Buffer = new Queue<ushort>();
            PressedKeys = new List<ushort>();
            ExtraMappings = new Dictionary<Keys,string>();
            ExtraMappings.Add(Keys.Oemcomma, ",");
            ExtraMappings.Add(Keys.OemPeriod, ".");
            ExtraMappings.Add(Keys.OemQuestion, "/");
            ExtraMappings.Add(Keys.Oem1, ";");
            ExtraMappings.Add(Keys.Oem7, "'");
            ExtraMappings.Add(Keys.OemOpenBrackets, "[");
            ExtraMappings.Add(Keys.Oem6, "]");
            ExtraMappings.Add(Keys.Oem5, "\"");
            ExtraMappings.Add(Keys.OemMinus, "-");
            ExtraMappings.Add(Keys.Oemplus, "=");
            ExtraMappings.Add(Keys.Oemtilde, "`");
            ExtraMappings.Add(Keys.Add, "+");
            ExtraMappings.Add(Keys.Subtract, "-");
            ExtraMappings.Add(Keys.Multiply, "*");
            ExtraMappings.Add(Keys.Divide, "/");
        }

        [Category("Device Information")]
        [TypeConverter(typeof(HexTypeEditor))]
        public override uint DeviceID
        {
            get { return 0x30cf7406; }
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
            get { return "Generic Keyboard (compatible)"; }
        }

        public override int HandleInterrupt()
        {
            switch (AttachedCPU.A)
            {
                case 0:
                    Buffer.Clear();
                    break;
                case 1:
                    if (Buffer.Count != 0)
                        AttachedCPU.C = Buffer.Dequeue();
                    else
                        AttachedCPU.C = 0;
                    break;
                case 2:
                    if (PressedKeys.Contains(AttachedCPU.B))
                        AttachedCPU.C = 1;
                    else
                        AttachedCPU.C = 0;
                    break;
                case 3:
                    InterruptMessage = AttachedCPU.B;
                    break;
            }
            return 0;
        }

        public override void Reset()
        {
            InterruptMessage = 0;
            Buffer = new Queue<ushort>();
        }

        public ushort GetKeyValue(Keys KeyCode)
        {
            switch (KeyCode)
            {
                case Keys.Up:
                    return 0x80;
                case Keys.Down:
                    return 0x81;
                case Keys.Left:
                    return 0x82;
                case Keys.Right:
                    return 0x83;
                case Keys.Back:
                    return 0x10;
                case Keys.Return:
                    return 0x11;
                case Keys.Insert:
                    return 0x12;
                case Keys.Delete:
                    return 0x13;
                case Keys.Control:
                    return 0x91;
                case Keys.ControlKey:
                    return 0x91;
                case Keys.Shift:
                case Keys.ShiftKey:
                    return 0x90;
                default:
                    string lowercase = "1234567890;\'-=`,./[]\\";
                    string uppercase = "!@#$%^&*():\"_+~<>?{}|";
                    ushort code = 0;
                    switch (KeyCode)
                    {
                        case Keys.NumPad0:
                            KeyCode = Keys.D0;
                            break;
                        case Keys.NumPad1:
                            KeyCode = Keys.D1;
                            break;
                        case Keys.NumPad2:
                            KeyCode = Keys.D2;
                            break;
                        case Keys.NumPad3:
                            KeyCode = Keys.D3;
                            break;
                        case Keys.NumPad4:
                            KeyCode = Keys.D4;
                            break;
                        case Keys.NumPad5:
                            KeyCode = Keys.D5;
                            break;
                        case Keys.NumPad6:
                            KeyCode = Keys.D6;
                            break;
                        case Keys.NumPad7:
                            KeyCode = Keys.D7;
                            break;
                        case Keys.NumPad8:
                            KeyCode = Keys.D8;
                            break;
                        case Keys.NumPad9:
                            KeyCode = Keys.D9;
                            break;
                    }
                    char key = Convert.ToChar(KeyCode);
                    if (ExtraMappings.ContainsKey(KeyCode))
                    {
                        key = ExtraMappings[KeyCode][0];
                        if (key == '"')
                            key = '\\';
                    }
                    string ascii = "?";
                    if (char.IsLetter(key))
                    {
                        ascii = key.ToString();
                        if (!PressedKeys.Contains(0x90))
                            ascii = ascii.ToLower();
                    }
                    else
                    {
                        if (lowercase.Contains(key))
                        {
                            if (!PressedKeys.Contains(0x90))
                                ascii = key.ToString();
                            else
                                ascii = uppercase[lowercase.IndexOf(key)].ToString();
                        }
                        else
                            ascii = key.ToString();
                    }
                    code = Encoding.ASCII.GetBytes(ascii)[0];
                    return code;
            }
        }

        public void KeyDown(Keys KeyCode)
        {
            ushort code = GetKeyValue(KeyCode);
            Buffer.Enqueue(code);
            if (!PressedKeys.Contains(code))
                PressedKeys.Add(code);
            if (InterruptMessage != 0)
                AttachedCPU.FireInterrupt(InterruptMessage);
        }

        public void KeyUp(Keys KeyCode)
        {
            ushort code = GetKeyValue(KeyCode);
            if (PressedKeys.Contains(code))
                PressedKeys.Remove(code);
        }
    }
}
