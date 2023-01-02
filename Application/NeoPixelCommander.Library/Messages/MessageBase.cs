using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.Library.Messages
{
    public abstract class MessageBase : IEquatable<MessageBase>
    {
        public abstract MessageType Type { get; }
        public abstract bool Equals(MessageBase other);
        public byte Red { get; protected set; }
        public byte Green { get; protected set; }
        public byte Blue { get; protected set; }
    }
}
