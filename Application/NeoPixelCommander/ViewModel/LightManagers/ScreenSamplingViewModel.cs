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
        }
                
        public int Interval
        {
            get => _screenSamplingManager.Interval;
            set
            {
                _screenSamplingManager.Interval = value;
                RaisePropertyChanged();
            }
        }

        public int Depth
        {
            get => _screenSamplingManager.Depth;
            set
            {
                _screenSamplingManager.Depth = value;
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
