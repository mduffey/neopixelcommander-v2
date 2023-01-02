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
    public class SingleMessage : MessageBase
    {
        public SingleMessage(Strip strip, byte index, byte red, byte green, byte blue)
        {
            var position = index << 2;
            Position = (byte)(position | (int)strip);
            Red = red;
            Green = green;
            Blue = blue;
        }

        public SingleMessage(Strip strip, byte index, Color color)
        {
            var position = index << 2;
            Position = (byte)(position | (int)strip);
            Red = color.R;
            Green = color.G;
            Blue = color.B;
        }
        public int Index => Position >> 2;
        public byte Position { get; }

        public override MessageType Type => MessageType.Single;

        public override bool Equals(MessageBase other)
        {
            var colorMatches = Red == other.Red && Blue == other.Blue && Green == other.Green;
            switch (other)
            {
                case SingleMessage single:
                    return single.Position == Position && colorMatches;
                case UniversalMessage _:
                    return colorMatches;
                default:
                    throw new NotImplementedException($"{other.GetType().Name} cannot be compared to a {nameof(SingleMessage)} message yet!");
            }
        }
    }
}
