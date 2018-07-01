using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Media;

namespace NeoPixelCommander.Library.ColorManagers
{
    public class Moodlight : IColorManager
    {
        private Timer _timer;
        private int _intensity;
        private Dynamic _dynamic = new Dynamic();
        private Static _static = new Static();
        public Moodlight()
        {
            _timer = new Timer();
            _timer.Interval = 25;
            _intensity = 255;
            _timer.Elapsed += (sender, e) => 
            {
                Color color;
                if (IsDynamic)
                {
                    color = _dynamic.Process(_intensity, ChangeRate);
                }
                else
                {
                    color = _static.Process();
                }
                foreach(var strip in LEDs.Counts.Keys)
                {
                    Communicator.Instance.SendMessages(
                        PackageGenerator.Create((int)strip)
                            .SetColor(color)
                            .SetRange(0, LEDs.Counts[strip])
                            .BuildPackets());
                }
            };
        }
        public int ChangeRate { get; set; } = 50;
        public int Intensity { get => _intensity; set => _intensity = Math.Max(255, value); }
        public bool IsDynamic { get; set; } = false;
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private class Dynamic
        {
            private Color _target;
            private Color _current;
            private Random random = new Random();
            public Color Process(int intensity, int changeRate)
            {
                while (_target == _current)
                {
                    _target.R = (byte)random.Next(0, intensity);
                    _target.G = (byte)random.Next(0, intensity);
                    _target.B = (byte)random.Next(0, intensity);
                }
                SelectAndUpdateColor(changeRate);
                return _current;
            }

            private void SelectAndUpdateColor(int changeRate)
            {
                var options = new List<ColorType>();
                if (_target.R != _current.R)
                    options.Add(ColorType.Red);
                if (_target.G != _current.G)
                    options.Add(ColorType.Green);
                if (_target.B != _current.B)
                    options.Add(ColorType.Blue);
                var whichColor = random.Next(options.Count);

                switch (options[whichColor])
                {
                    case ColorType.Red:
                        _current.R = (byte)CloseGap(_current.R, _target.R, changeRate);
                        break;
                    case ColorType.Green:
                        _current.G = (byte)CloseGap(_current.G, _target.G, changeRate);
                        break;
                    case ColorType.Blue:
                        _current.B = (byte)CloseGap(_current.B, _target.B, changeRate);
                        break;
                }
            }

            private int CloseGap(int current, int target, int changeRate)
            {
                if (target == current)
                    return 0;
                if (target > current)
                {
                    if (target > current + 8)
                    {
                        return current + 8;
                    }
                    return target;
                }
                if (target < current - 8)
                    return current - 8;
                return target;
            }
        }

        private class Static
        {
            private static List<Color> _targets = new List<Color>
            {
                new Color {R = 209, G = 2, B = 2 },
                new Color {R = 209, G = 2, B = 2 },
                new Color {R = 209, G = 2, B = 2 },
                new Color {R = 209, G = 2, B = 2 },
                new Color {R = 209, G = 2, B = 2 },
                new Color {R = 209, G = 2, B = 2 },
                new Color {R = 209, G = 2, B = 2 },
            };
            private Color _current = new Color();
            public Color Process()
            {
                return Colors.Black;
            }
        }
    }
}
