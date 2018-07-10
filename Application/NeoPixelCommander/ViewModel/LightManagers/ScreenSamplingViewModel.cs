using GalaSoft.MvvmLight;
using NeoPixelCommander.Library.ColorManagers;
using NeoPixelCommander.Properties;
using System.Windows.Input;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class ScreenSamplingViewModel : ViewModelBase, ILightManager, IActiveLightManager
    {
        public string Name => "Screen Sampling";

        private ScreenSamplingManager _screenSamplingManager;

        public ScreenSamplingViewModel(ScreenSamplingManager screenSamplingManager)
        {
            _screenSamplingManager = screenSamplingManager;
            StartCommand = new Command((o) => true, (o) => Start());
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

        public ICommand StartCommand { get; }
        
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
