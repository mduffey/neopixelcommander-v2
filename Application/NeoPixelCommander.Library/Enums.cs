using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.Library
{
    public enum ColorType { Red, Green, Blue }

    public enum Strip
    {
        Left = 1,
        Right = 0,
        Top = 3,
        Bottom = 2
    }

    public enum LogLevel
    {
        /// <summary>
        /// Unknown means we were unable to query the device.
        /// </summary>
        Unknown = 0,
        Verbose = 10,
        Actions = 20,
        Unexpected = 30,
        None = 40
    }

    public enum Availability
    {
        Unknown = 0,
        Disconnected = 1,
        /// <summary>
        /// The device has disabled updates. Will likely be because of a potentionmeter that will be added...eventually.
        /// </summary>
        Disabled = 100,
        Online = 200
    }
}
