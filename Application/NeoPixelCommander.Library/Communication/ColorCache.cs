using NeoPixelCommander.Library.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.Library.Communication
{
    public class ColorCache : IColorCache
    {
        private readonly PackageHandler _packageHandler;
        private readonly Dictionary<byte, MessageBase> _lastMessageByLED;

        public ColorCache(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
            _lastMessageByLED = new Dictionary<byte, MessageBase>();

            for(var strip = 0; strip < LEDs.Counts.Count; strip++)
            {
                for(var led = 0; led < LEDs.Counts[(Strip)strip]; led++)
                {
                    var position = led << 2;
                    _lastMessageByLED.Add((byte)(position | strip), null);
                }
            }
        }

        public void SendRange(ICollection<SingleMessage> rangeMessages)
        {
            var messagesToSend = rangeMessages.Where(m =>
            {
                var lastMessage = _lastMessageByLED[m.Position];

                // If no message has been set, we're definitely sending this.
                if (lastMessage == null)
                {
                    return true;
                }

                return !lastMessage.Equals(m);
            }).ToList();

            foreach(var message in messagesToSend)
            {
                _lastMessageByLED[message.Position] = message;
            }

            _packageHandler.SendRange(messagesToSend);
        }

        public void Reset()
        {
            foreach(var key in _lastMessageByLED.Keys.ToList())
            {
                _lastMessageByLED[key] = null;
            }
        }
    }
}
