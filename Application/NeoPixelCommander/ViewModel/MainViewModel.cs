using GalaSoft.MvvmLight;
using Settings = NeoPixelCommander.Properties.Settings;
using System.Collections.Generic;
using System.Linq;
using NeoPixelCommander.ViewModel.LightManagers;
using NeoPixelCommander.Library;
using Microsoft.Win32;

namespace NeoPixelCommander.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly List<ILightManager> _availableManagers;
        private readonly PackageHandler _packageHandler;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(StatusViewModel status, IEnumerable<ILightManager> managers, PackageHandler packageHandler)
        {
            if (IsInDesignMode)
            {
                _availableManagers = new List<ILightManager>
                {

                };
            }
            else
            {

                _availableManagers = managers.ToList();
                _packageHandler = packageHandler;
                SystemEvents.PowerModeChanged += PowerModeChanged;
                SystemEvents.SessionSwitch += SessionSwitched;
                SystemEvents.SessionEnded += SessionEnded;
                var manager = _availableManagers.FirstOrDefault(m => m.Name == Settings.Default.Main_CurrentManager);

                _selectedManager = manager ?? _availableManagers.First();
                if (_selectedManager is IAutomaticLightManager activeLightManager)
                {
                    activeLightManager.Start();
                }
                Status = status;
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            SystemEvents.PowerModeChanged -= PowerModeChanged;
            SystemEvents.SessionSwitch -= SessionSwitched;
        }

        private void SessionEnded(object sender, SessionEndedEventArgs e)
        {
            (_selectedManager as IActiveLightManager)?.Stop();
            _packageHandler.SendUniversal(new System.Windows.Media.Color { R = 0, G = 0, B = 0 });
        }

        private void SessionSwitched(object sender, SessionSwitchEventArgs e)
        {
            var manager = _selectedManager as IActiveLightManager;
            var autoManager = _selectedManager as IAutomaticLightManager;
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                if (manager != null)
                {
                    manager.Start();
                }
                else
                {
                    autoManager?.Start();
                }
            }
        }
        
        private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            var manager = _selectedManager as IActiveLightManager;
            if (e.Mode == PowerModes.Suspend)
            {
                manager?.Stop();
                _packageHandler.SendUniversal(new System.Windows.Media.Color { R = 255, G = 20, B = 20 });
            }
        }

        public List<ILightManager> AvailableManagers => _availableManagers;

        private ILightManager _selectedManager;
        public ILightManager SelectedManager
        {
            get => _selectedManager;
            set
            {
                if (_selectedManager.Name != value.Name)
                {
                    if (_selectedManager is IActiveLightManager activeLightManager)
                    {
                        activeLightManager.Stop();
                    }
                    _selectedManager = value;
                    Settings.Default.Main_CurrentManager = value.Name;
                    if (_selectedManager is IAutomaticLightManager automaticLightManager)
                    {
                        automaticLightManager.Start();
                    }
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(CanPause));
                }
            }
        }

        public StatusViewModel Status { get; }

        public bool CanPause => _selectedManager is IActiveLightManager;

        private bool _pause;
        public bool Pause
        {
            get => _pause;
            set
            {
                if (_pause != value && _selectedManager is IActiveLightManager lightManager)
                {
                    _pause = value;
                    if (_pause)
                    {
                        lightManager.Stop();
                    }
                    else
                    {
                        lightManager.Start();
                    }
                }
            }
        }
    }
}