using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library
{
    public enum Strip
    {
        Left = 1,
        Right = 0,
        Top = 3,
        Bottom = 2
    }

    public enum MessageType
    {
        /// <summary>
        /// Send individual strip and the LED to set, and the colors to which to set it.
        /// </summary>
        Range = 10,
        /// <summary>
        /// Define the strip and send one color. Will be applied on all LEDs on that strip.
        /// </summary>
        Strip = 20,
        /// <summary>
        /// Send one color, which will be applied across all LEDs.
        /// </summary>
        Universal = 30,
        /// <summary>
        /// Send two colors in the packet; the Teensy will handle defining the gradient.
        /// </summary>
        Gradient = 40
    }


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

        public byte Position { get; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
    }

    public static class LEDs
    {
        
        private static Dictionary<Strip, int> _counts = new Dictionary<Strip, int>
        {
            {Strip.Left, 20 },
            {Strip.Right, 20 },
            {Strip.Top, 43 },
            {Strip.Bottom, 43 },
        };

        public static Dictionary<Strip, int> Counts => _counts;
        
        // Note: all messages define an initial byte with a value of 0, as it's swallowed by...something. Appears to be a standard
        // HID thing, as the Teensy reports being able to receive 65 byte packets, even though we can only send 64.
        public static bool SendUniversal(Color color)
        {
            var bytes = new byte[5];
            bytes[0] = 0;
            bytes[1] = (byte)MessageType.Universal;
            bytes[2] = color.R;
            bytes[3] = color.G;
            bytes[4] = color.B;
            return Communicator.Instance.SendMessage(bytes);
        }


        public static void SendRange(ICollection<RangeMessage> rangeMessages)
        {
            int count = 0;
            var bytes = new byte[65];
            bytes[0] = 0;
            foreach(var message in rangeMessages)
            {
                bytes[count * 4 + 1] = message.Position;
                bytes[count * 4 + 2] = message.Red;
                bytes[count * 4 + 3] = message.Green;
                bytes[count * 4 + 4] = message.Blue;
                count++;
                if (count == 16)
                {
                    Communicator.Instance.SendMessage(bytes);
                    count = 0;
                }
            }
            if (count < 16)
            {
                // Terminator message
                bytes[count * 4 + 1] = byte.MaxValue;
            }
        }
    }

    
}
