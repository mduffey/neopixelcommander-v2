using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Extensions
{
    public static class ColorExtensions
    {
        public static System.Drawing.Color ToDrawing(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        public static Color ToMedia(this System.Drawing.Color color)
        {
            return new Color
            {
                R = color.R,
                G = color.G,
                B = color.B
            };
        }
    }
}
