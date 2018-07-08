using GalaSoft.MvvmLight;
using NeoPixelCommander.Extensions;
using NeoPixelCommander.Library;
using Settings = NeoPixelCommander.Properties.Settings;
using System.Windows.Media;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class SingleColorViewModel : ViewModelBase, ILightManager, IAutomaticLightManager
    {
        private readonly PackageHandler _packageHandler;

        public SingleColorViewModel(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
            var settingsColor = Settings.Default.SingleColor_Color;
            _color = settingsColor != null
                ? settingsColor.ToMedia()
                : Colors.Red;
        }
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
                    Settings.Default.SingleColor_Color = _color.ToDrawing();
                    RaisePropertyChanged();
                }
            }
        }

        public void Start()
        {
            _packageHandler.SendUniversal(_color);
        }
    }
}
