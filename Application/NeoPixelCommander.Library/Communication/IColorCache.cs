using NeoPixelCommander.Library.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.Library.Communication
{
    public interface IColorCache
    {
        /// <summary>
        /// Same as talking to <see cref="PackageHandler"/>, but it tracks messages sent by position and filters out any messages that are the same as previous messages.
        /// </summary>
        /// <param name="rangeMessages"></param>
        void SendRange(ICollection<SingleMessage> rangeMessages);

        /// <summary>
        /// Wipe all messages, when switching off of ScreenSampling.
        /// </summary>
        void Reset();
    }
}
