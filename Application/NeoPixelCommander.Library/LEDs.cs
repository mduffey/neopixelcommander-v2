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
            {Strip.Left, 36 },
            {Strip.Right, 36 },
            {Strip.Top, 61 },
            {Strip.Bottom, 61 }
        };
    }
}