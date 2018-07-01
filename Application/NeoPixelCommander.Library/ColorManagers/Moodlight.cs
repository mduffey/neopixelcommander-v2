using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Media;

namespace NeoPixelCommander.Library.ColorManagers
{
    public class Moodlight : IColorManager
    {
        private Timer _timer;
        private int _intensity, _changeRate, _interval;
        private Dynamic _dynamic = new Dynamic();
        public Moodlight()
        {
            _timer = new Timer();
            _intensity = 255;
            _changeRate = 4;
            _interval = 25;
            _timer.Elapsed += (sender, e) => 
            {
                var color = _dynamic.Process(_intensity, _changeRate);
                LEDs.SendUniversal(color);
            };
        }
        public int Intensity
        {
            get => _intensity;
            set
            {
                var actual = Math.Min(255, value);
                if (actual > 0 && actual != _intensity)
                {
                    _intensity = actual;
                }
            }
        }
        
        public int Interval
        {
            get => _interval;
            set
            {
                var actual = Math.Min(250, value);
                if (actual > 0 && actual != _interval)
                {
                    _interval = actual;
                    _timer.Interval = _interval;
                }
            }
        }

        public int ChangeRate
        {
            get => _changeRate;
            set
            {
                var actual = Math.Min(8, value);
                if (actual > 0 && actual != _changeRate)
                {
                    _changeRate = actual;
                }
            }
        }

        public void Start()
        {
            _timer.Interval = _interval;
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
                    if (target > current + changeRate)
                    {
                        return current + changeRate;
                    }
                    return target;
                }
                if (target < current - changeRate)
                    return current - changeRate;
                return target;
            }
        }
    }
}
