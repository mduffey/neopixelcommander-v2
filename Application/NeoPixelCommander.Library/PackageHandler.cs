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

        public bool SendUniversal(Color color)
        {
            var message = new byte[5];
            message[0] = (int)MessageType.Universal;
            message[1] = color.R;
            message[2] = color.G;
            message[3] = color.B;
            message[4] = 0;
            _communicator.SendMessage(message);
            return true;
        }

        public bool SendSettings(LogLevel logLevel)
        {
            return true; // Going away
        }

        // Forces an update of status, for when we alter log level (so we can get the message back immediately).
        public void GetStatus()
        {
            _communicator.GetStatus();
        }

        public void SendRange(ICollection<SingleMessage> rangeMessages)
        {
            if (rangeMessages.Any())
            {
                var count = 0;
                var bytes = new byte[rangeMessages.Count * 5];
                foreach (var message in rangeMessages)
                {
                    bytes[count * 5] = (byte)message.Type;
                    bytes[count * 5 + 1] = message.Position;
                    bytes[count * 5 + 2] = message.Red;
                    bytes[count * 5 + 3] = message.Green;
                    bytes[count * 5 + 4] = message.Blue;
                    count++;
                }

                if (count != 0)
                {
                    _communicator.SendMessage(bytes);
                }
            }
        }

        public void SendRange(params SingleMessage[] rangeMessages)
        {
            SendRange(rangeMessages);
        }

    }
}
