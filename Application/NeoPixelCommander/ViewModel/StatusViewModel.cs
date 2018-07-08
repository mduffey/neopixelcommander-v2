using GalaSoft.MvvmLight;
using NeoPixelCommander.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoPixelCommander.ViewModel
{
    public class StatusViewModel: ViewModelBase
    {
        private readonly IStatus _status;
        private readonly PackageHandler _packageHandler;
        private readonly ICollection<LogLevel> _availableLogLevels;

        public StatusViewModel(IStatus status, PackageHandler packageHandler)
        {
            _status = status;
            _status.AvailabilityChanged += (sender, e) => RaisePropertyChanged(nameof(Availability));
            _status.LogLevelChanged += (sender, e) => RaisePropertyChanged(nameof(LogLevel));
            _packageHandler = packageHandler;
            _availableLogLevels = Enum.GetValues(typeof(LogLevel))
                .Cast<LogLevel>().Where(l => l != LogLevel.Unknown).ToList();
        }

        public Availability Availability => _status.Availability;
        
        public LogLevel LogLevel
        {
            get => _status.LogLevel;
            set
            {
                _packageHandler.SendSettings(value);
                RaisePropertyChanged();
            }
        }

        public IEnumerable<LogLevel> AvailableLogLevels => _availableLogLevels;
    }
}
