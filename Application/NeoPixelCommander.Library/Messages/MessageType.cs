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
        Gradient = 40,
        /// <summary>
        /// Requests that the device send a current status report. Currently, that's logging level and whether the device is on.
        /// </summary>
        Status = 100,
        /// <summary>
        /// Update the current settings for how the Teensy operates. For now, adjusts the Serial logging frequency.
        /// </summary>
        Settings = 110
    }
}
