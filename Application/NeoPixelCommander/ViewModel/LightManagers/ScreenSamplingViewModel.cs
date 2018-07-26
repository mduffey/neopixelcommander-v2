using GalaSoft.MvvmLight;
using NeoPixelCommander.Library.ColorManagers;
using NeoPixelCommander.Properties;
using System.Windows.Input;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class ScreenSamplingViewModel : ViewModelBase, ILightManager, IActiveLightManager, IAutomaticLightManager
    {
        public string Name => "Screen Sampling";

        private ScreenSamplingManager _screenSamplingManager;

        public ScreenSamplingViewModel(ScreenSamplingManager screenSamplingManager)
        {
            _screenSamplingManager = screenSamplingManager;
            _screenSamplingManager.Depth = Settings.Default.ScreenSampling_Depth;
            _screenSamplingManager.Interval = Settings.Default.ScreenSampling_Interval;
            _screenSamplingManager.Saturation = Settings.Default.ScreenSampling_Saturation;
        }
                
        public int Interval
        {
            get => _screenSamplingManager.Interval;
            set
            {
                _screenSamplingManager.Interval = value;
                Settings.Default.ScreenSampling_Interval = value;
                RaisePropertyChanged();
            }
        }

        public int Depth
        {
            get => _screenSamplingManager.Depth;
            set
            {
                _screenSamplingManager.Depth = value;
                Settings.Default.ScreenSampling_Depth = value;
                RaisePropertyChanged();
            }
        }

        public int Saturation
        {
            get => _screenSamplingManager.Saturation;
            set
            {
                _screenSamplingManager.Saturation = value;
                Settings.Default.ScreenSampling_Saturation = value;
                RaisePropertyChanged();
            }
        }

        public void Start()
        {
            _screenSamplingManager.Start();
        }

        public void Stop()
        {
            _screenSamplingManager.Stop();
        }
    }

    
}
