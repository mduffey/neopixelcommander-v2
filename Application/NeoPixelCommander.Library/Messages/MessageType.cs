using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.Library.Messages
{
    public enum MessageType
    {
        /// <summary>
        /// Send a request to verify that we're talking to the right device
        /// </summary>
        Check = 10,
        /// <summary>
        /// Send one LED's colors
        /// </summary>
        Single = 20,
        /// <summary>
        /// Send one color to all LEDs
        /// </summary>
        Universal = 30
    }
}
