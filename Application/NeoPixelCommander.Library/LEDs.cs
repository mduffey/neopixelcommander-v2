using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library
{
    public static class LEDs
    {
        public static IReadOnlyDictionary<Strip, int> Counts => new Dictionary<Strip, int>
        {
            {Strip.Left, 20 },
            {Strip.Right, 20 },
            {Strip.Top, 43 },
            {Strip.Bottom, 43 },
        };
    }
}