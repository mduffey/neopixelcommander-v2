namespace NeoPixelCommander.Library
{
    public enum LogLevel
    {
        Verbose = 0,
        Actions = 10,
        Unexpected = 20,
        None = 30
    }
    public class Settings
    {
        private readonly PackageHandler _packageHandler;

        public Settings(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
        }
        private LogLevel _currentLogLevel;
        public LogLevel CurrentLogLevel {
            get => _currentLogLevel;
            set
            {
                _currentLogLevel = value;
                _packageHandler.SendSettings(_currentLogLevel);
            }
        }
    }
}
