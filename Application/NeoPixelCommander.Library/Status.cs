using System;
using System.Timers;

namespace NeoPixelCommander.Library
{
    public interface IStatus
    {
        LogLevel LogLevel { get; }
        Availability Availability { get; }
        event EventHandler LogLevelChanged;
        event EventHandler AvailabilityChanged;
    }

    public interface IUpdateStatus
    {
        LogLevel LogLevel { get;  set; }
        Availability Availability { get;  set; }
    }

    public class Status: IStatus, IUpdateStatus
    {
        private LogLevel _logLevel;
        public LogLevel LogLevel
        {
            get => _logLevel;
            set
            {
                if (_logLevel != value)
                {
                    _logLevel = value;
                }
                LogLevelChanged?.Invoke(this, new EventArgs());
            }
        }

        private Availability _availability;
        public Availability Availability
        {
            get => _availability;
            set
            {
                if (_availability != value)
                {
                    _availability = value;
                }
                AvailabilityChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler AvailabilityChanged;
        public event EventHandler LogLevelChanged;
    }
}
