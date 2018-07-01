using GalaSoft.MvvmLight;
using NeoPixelCommander.Library.ColorManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.ViewModel
{
    public class MoodlightViewModel: ViewModelBase, ILightManager
    {
        public string Name => "Moodlight";
        private Moodlight _moodlight;
        public MoodlightViewModel()
        {
            _moodlight = new Moodlight();
        }

        public int Intensity
        {
            get => _moodlight.Intensity;
            set
            {
                if (_moodlight.Intensity != value)
                {
                    _moodlight.Intensity = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public int Interval
        {
            get => _moodlight.Interval;
            set
            {
                if (_moodlight.Interval != value)
                {
                    _moodlight.Interval = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ChangeRate
        {
            get => _moodlight.ChangeRate;
            set
            {
                if (_moodlight.ChangeRate != value)
                {
                    _moodlight.ChangeRate = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public bool Dynamic
        {
            get => _moodlight.IsDynamic;
            set
            {
                if(_moodlight.IsDynamic != value)
                {
                    _moodlight.IsDynamic = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void Start()
        {
            _moodlight.Start();
        }

        public void Stop()
        {
            _moodlight.Stop();
        }
    }

    
}
