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
            _interval = 50;
            StartCommand = new Command((o) => true, (o) => Start());
        }
        
        private int _interval;
        
        public int Interval
        {
            get => _interval;
            set
            {
                _interval = value;
                RaisePropertyChanged();
            }
        }

        public ICommand StartCommand { get; }
        
        public void Start()
        {
            _screenSamplingManager.Start(_interval);
        }

        public void Stop()
        {
            _screenSamplingManager.Stop();
        }
    }

    
}
