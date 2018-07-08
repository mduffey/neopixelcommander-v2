using NeoPixelCommander.Library.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library.ColorManagers
{
    public class GradientManager
    {
        private PackageHandler _packageHandler;
        public GradientManager(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
        }

        private Color _firstColor;
        public Color FirstColor
        {
            get => _firstColor;
            set
            {
                if (_firstColor != value)
                {
                    _firstColor = value;
                }
            }
        }

        private Color _secondColor;
        public Color SecondColor
        {
            get => _secondColor;
            set
            {
                if (_secondColor != value)
                {
                    _secondColor = value;
                }
            }
        }

        public void Send()
        {
            SendGradient(FirstColor, SecondColor);
        }

        private void SendGradient(Color first, Color second)
        {
            var horizontalColors = Calculate(first, second, LEDs.Counts[Strip.Top] * 2);
            var verticalColors = Calculate(first, second, LEDs.Counts[Strip.Left] * 2);
            verticalColors.Reverse();
            var messages = new List<RangeMessage>();
            for (int i = 0; i < LEDs.Counts[Strip.Top]; i++)
            {
                messages.Add(new RangeMessage(Strip.Top, (byte)i, horizontalColors[i]));
                messages.Add(new RangeMessage(Strip.Bottom, (byte)i, horizontalColors[i + LEDs.Counts[Strip.Top]]));
            }
            for (int i = LEDs.Counts[Strip.Left] - 1; i >= 0; i--)
            {
                messages.Add(new RangeMessage(Strip.Left, (byte)i, verticalColors[i + LEDs.Counts[Strip.Right]]));
                messages.Add(new RangeMessage(Strip.Right, (byte)i, verticalColors[i]));
            }
            _packageHandler.SendRange(messages);
        }

        // Thanks! https://jsfiddle.net/002v98LL/286/
        private static List<Color> Calculate(Color first, Color second, int steps)
        {
            var factor = 1d / (steps - 1);
            var result = new List<Color>();
            for (var i = 0; i < steps; i++)
            {
                result.Add(GetColor(first, second, factor * i));
            }
            return result;
        }

        private static Color GetColor(Color first, Color second, double factor)
        {
            return new Color()
            {
                R = Calculate(first.R, second.R, factor),
                G = Calculate(first.G, second.G, factor),
                B = Calculate(first.B, second.B, factor)
            };
        }

        private static byte Calculate(int first, int second, double factor)
        {
            return (byte)Math.Round(first + factor * (second - first), 0);
        }
    }
}
