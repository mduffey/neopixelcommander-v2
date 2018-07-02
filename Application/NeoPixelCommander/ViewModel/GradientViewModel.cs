using GalaSoft.MvvmLight;
using NeoPixelCommander.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.ViewModel
{
    public class GradientViewModel: ViewModelBase, ILightManager
    {
        public string Name => "Gradient";

        public GradientViewModel()
        {
            _startColor = Colors.Red;
            _stopColor = Colors.White;
        }
        private Color _startColor, _stopColor;

        public Color StartColor
        {
            get => _startColor;
            set
            {
                if (_startColor != value)
                {
                    _startColor = value;
                    Start();
                    RaisePropertyChanged();
                }
            }
        }

        public Color StopColor
        {
            get => _stopColor;
            set
            {
                if (_stopColor != value)
                {
                    _stopColor = value;
                    Start();
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsActive => false;

        public void Start()
        {
            LEDs.SendGradient(_startColor, _stopColor);
        }

        public void Stop()
        {
            // Only relevant for active color managers.
        }
    }
}
