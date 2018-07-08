using GalaSoft.MvvmLight;
using NeoPixelCommander.Library.ColorManagers;
using NeoPixelCommander.Properties;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class MoodlightViewModel: ViewModelBase, ILightManager, IActiveLightManager, IAutomaticLightManager
    {
        public string Name => "Moodlight";

        private MoodlightManager _moodlightManager;

        public MoodlightViewModel(MoodlightManager moodlightManager)
        {
            _moodlightManager = moodlightManager;
            _moodlightManager.Intensity = Settings.Default.Moodlight_Intensity;
            _moodlightManager.ChangeRate = Settings.Default.Moodlight_ChangeRate;
            _moodlightManager.Interval = Settings.Default.Moodlight_Interval;
        }

        public int Intensity
        {
            get => _moodlightManager.Intensity;
            set
            {
                if (_moodlightManager.Intensity != value)
                {
                    _moodlightManager.Intensity = value;
                    Settings.Default.Moodlight_Intensity = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public int Interval
        {
            get => _moodlightManager.Interval;
            set
            {
                if (_moodlightManager.Interval != value)
                {
                    _moodlightManager.Interval = value;
                    Settings.Default.Moodlight_Interval = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ChangeRate
        {
            get => _moodlightManager.ChangeRate;
            set
            {
                if (_moodlightManager.ChangeRate != value)
                {
                    _moodlightManager.ChangeRate = value;
                    Settings.Default.Moodlight_ChangeRate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void Start()
        {
            _moodlightManager.Start();
        }

        public void Stop()
        {
            _moodlightManager.Stop();
        }
    }

    
}
