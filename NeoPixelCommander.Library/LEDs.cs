using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.Library
{
    public enum Strip
    {
        Left = 1,
        Right = 0,
        Top = 3,
        Bottom = 2,
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
    }
}
