using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace Tomato
{
    public class HexTypeEditor : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string text = value as string;
            if (context.PropertyDescriptor.PropertyType == typeof(uint))
            {
                if (text.StartsWith("0x"))
                    return uint.Parse(text.Substring(2), NumberStyles.HexNumber);
                else
                    return uint.Parse(text);
            }
            else
            {
                if (text.StartsWith("0x"))
                    return ushort.Parse(text.Substring(2), NumberStyles.HexNumber);
                else
                    return ushort.Parse(text);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is uint)
                return "0x" + GetHexString((uint)value, 8);
            else
                return "0x" + GetHexString((ushort)value, 4);
        }

        public static string GetHexString(uint value, int numDigits)
        {
            string result = value.ToString("x").ToUpper();
            while (result.Length < numDigits)
                result = "0" + result;
            return result;
        }
    }
}
