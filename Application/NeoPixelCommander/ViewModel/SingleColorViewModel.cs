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
    public class SingleColorViewModel : ViewModelBase, ILightManager
    {
        public string Name => "Single Color";

        private Color _color;

        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    Start();
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsActive => false;

        public void Start()
        {
            LEDs.SendUniversal(_color);
        }

        public void Stop()
        {
            // Only relevant for active color managers.
        }
    }
}
