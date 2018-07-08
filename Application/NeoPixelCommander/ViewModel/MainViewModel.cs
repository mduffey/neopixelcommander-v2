using GalaSoft.MvvmLight;
using Settings = NeoPixelCommander.Properties.Settings;
using System.Collections.Generic;
using System.Linq;
using NeoPixelCommander.ViewModel.LightManagers;
using NeoPixelCommander.Library;

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
        
        private List<ILightManager> _availableManagers;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(StatusViewModel status,IEnumerable<ILightManager> managers = null)
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
                var manager = _availableManagers.FirstOrDefault(m => m.Name == Settings.Default.Main_CurrentManager);

                _selectedManager = manager ?? _availableManagers.First();
                if (_selectedManager is IActiveLightManager activeLightManager)
                {
                    activeLightManager.Start();
                }
                Status = status;
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
                    if (_selectedManager is IActiveLightManager newActiveLightManager)
                    {
                        newActiveLightManager.Start();
                    }
                    else if (_selectedManager is IAutomaticLightManager automaticLightManager)
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