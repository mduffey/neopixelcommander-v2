using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;

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
        public MainViewModel(IEnumerable<ILightManager> managers = null)
        {
            if (IsInDesignMode)
            {
                _availableManagers = new List<ILightManager>
                {
                    new MoodlightViewModel()
                };
            }
            else
            {
                _availableManagers = managers.ToList();
            }
        }

        public List<ILightManager> AvailableManagers => _availableManagers;

        private ILightManager _activeManager;
        public ILightManager ActiveManager
        {
            get => _activeManager;
            set
            {
                if (_activeManager == null || _activeManager.Name != value.Name)
                {
                    _activeManager?.Stop();
                    _activeManager = value;
                    _activeManager.Start();
                    RaisePropertyChanged();
                }
            }
        }

        public string Name => "hi";
    }
}