using GalaSoft.MvvmLight;
using NeoPixelCommander.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.ViewModel
{
    public class SettingsViewModel: ViewModelBase
    {
        private Settings _settings;
        
        public LogLevel LogLevel
        {
            get => _settings.CurrentLogLevel;
            set
            {
                _settings.CurrentLogLevel = value;
                RaisePropertyChanged();
            }
        }
    }
}
