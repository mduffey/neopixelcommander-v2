using NeoPixelCommander.Library.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library
{
    public class PackageHandler
    {
        private Communicator _communicator;
        public PackageHandler(Communicator communicator)
        {
            _communicator = communicator;
        }
        // Note: all messages define an initial byte with a value of 0, as it's swallowed by...something. Appears to be a standard
        // HID thing, as the Teensy reports being able to receive 65 byte packets, even though we can only send 64.
        public bool SendUniversal(Color color)
        {
            var bytes = CreatePacket(MessageType.Universal);
            bytes[2] = color.R;
            bytes[3] = color.G;
            bytes[4] = color.B;
            return _communicator.SendMessage(bytes);
        }

        public bool SendSettings(LogLevel logLevel)
        {
            var bytes = CreatePacket(MessageType.Settings);
            bytes[2] = (byte)logLevel;
            return _communicator.SendMessage(bytes);
        }

        public void SendRange(IEnumerable<RangeMessage> rangeMessages)
        {
            int count = 0;
            var bytes = CreatePacket(MessageType.Range);
            foreach (var message in rangeMessages)
            {
                bytes[count * 4 + 2] = message.Position;
                bytes[count * 4 + 3] = message.Red;
                bytes[count * 4 + 4] = message.Green;
                bytes[count * 4 + 5] = message.Blue;
                count++;
                if (count == 15)
                {
                    _communicator.SendMessage(bytes);
                    bytes = CreatePacket(MessageType.Range);
                    count = 0;
                }
            }
            if (count != 0)
            {
                // Terminator message
                bytes[count * 4 + 2] = byte.MaxValue;
                _communicator.SendMessage(bytes);
            }
        }

        public void SendRange(params RangeMessage[] rangeMessages)
        {
            SendRange(rangeMessages);
        }

        private byte[] CreatePacket(MessageType messageType)
        {
            var bytes = new byte[65];
            bytes[0] = 0;
            bytes[1] = (byte)messageType;
            // This should be the highest value we can send in a packet, so we preset it
            // with the terminator message.
            bytes[62] = byte.MaxValue;

            return bytes;
        }

    }
}
