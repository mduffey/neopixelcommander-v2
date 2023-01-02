using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library.Messages
{
    public class UniversalMessage : MessageBase
    {
        public override MessageType Type => MessageType.Universal;

        public UniversalMessage(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public UniversalMessage(Color color)
        {
            Red = color.R;
            Green = color.G;
            Blue = color.B;
        }

        public override bool Equals(MessageBase other)
        {
            return other is UniversalMessage universal && universal.Red == Red && universal.Blue == Blue && universal.Green == Green;
        }
    }
}
