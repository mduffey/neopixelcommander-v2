using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library.Messages
{
    /// <summary>
    /// Represents the mini-packet that defines the message for a single LED on a single strip.
    /// </summary>
    public struct RangeMessage
    {
        public RangeMessage(Strip strip, byte index, byte red, byte green, byte blue)
        {
            var position = index << 2;
            Position = (byte)(position | (int)strip);
            Red = red;
            Green = green;
            Blue = blue;
        }

        public RangeMessage(Strip strip, byte index, Color color)
        {
            var position = index << 2;
            Position = (byte)(position | (int)strip);
            Red = color.R;
            Green = color.G;
            Blue = color.B;
        }
        public int Index => Position >> 2;
        public byte Position { get; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
    }
}
