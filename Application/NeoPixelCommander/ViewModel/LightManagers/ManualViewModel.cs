using GalaSoft.MvvmLight;
using NeoPixelCommander.Library;
using NeoPixelCommander.Library.ColorManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class ManualViewModel : ViewModelBase, ILightManager
    {
        public string Name => "Manual Control";
        private ManualManager _manualManager;

        public ManualViewModel(ManualManager manualManager)
        {
            _manualManager = manualManager;
            _manualManager.Color = Colors.Purple;
            _manualManager.SingleLED = 1;
            _manualManager.FirstLED = 1;
            _manualManager.LastLED = 1;
            Update = new Command((o) => AreAnyStripsSelected, Update_Execute);
        }
        
        public Color Color
        {
            get => _manualManager.Color;
            set
            {
                _manualManager.Color = value;
                RaisePropertyChanged();
            }
        }

        public bool UpdateLeft
        {
            get => _manualManager.SelectedStrips.Contains(Strip.Left);
            set => UpdateStrip(Strip.Left, value);
        }

        public bool UpdateRight
        {
            get => _manualManager.SelectedStrips.Contains(Strip.Right);
            set => UpdateStrip(Strip.Right, value);
        }

        public bool UpdateTop
        {
            get => _manualManager.SelectedStrips.Contains(Strip.Top);
            set => UpdateStrip(Strip.Top, value);
        }

        public bool UpdateBottom
        {
            get => _manualManager.SelectedStrips.Contains(Strip.Bottom);
            set => UpdateStrip(Strip.Bottom, value);
        }

        private void UpdateStrip(Strip strip, bool value, [CallerMemberName] string name = "")
        {
            if (value)
            {
                if (!_manualManager.SelectedStrips.Contains(strip))
                {
                    _manualManager.SelectedStrips.Add(strip);
                }
            }
            else
            {
                if (_manualManager.SelectedStrips.Contains(strip))
                {
                    _manualManager.SelectedStrips.Remove(strip);
                }
            }
            RaisePropertyChanged(name);
            RaisePropertyChanged(nameof(Maximum));
            RaisePropertyChanged(nameof(AreAnyStripsSelected));
            RaisePropertyChanged(nameof(LEDsSelectedExceedRangeForSomeStrips));
        }
        
        public int StartLED
        {
            get => _manualManager.FirstLED;
            set
            {
                _manualManager.FirstLED = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LEDsSelectedExceedRangeForSomeStrips));
            }
        }

        public int StopLED
        {
            get => _manualManager.LastLED;
            set
            {
                _manualManager.LastLED = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LEDsSelectedExceedRangeForSomeStrips));
            }
        }
        
        public int SingleLED
        {
            get => _manualManager.SingleLED;
            set
            {
                _manualManager.SingleLED = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LEDsSelectedExceedRangeForSomeStrips));
            }
        }

        public int Maximum
        {
            get
            {
                if (!_manualManager.SelectedStrips.Any())
                {
                    return 1;
                }
                return LEDs.Counts.Where(kvp => _manualManager.SelectedStrips.Contains(kvp.Key)).Max(kvp => kvp.Value);
            }
        }
        
        public bool ShouldUpdateSingleLED
        {
            get => _manualManager.UsingSingleLED;
            set
            {
                _manualManager.UsingSingleLED = value;
                RaisePropertyChanged();
            }
        }

        // If no strips are selected, there's nothing to enable.
        public bool AreAnyStripsSelected => _manualManager.SelectedStrips.Any();

        public bool LEDsSelectedExceedRangeForSomeStrips => _manualManager.SelectedStrips.Any(ExceedsMaxForStrip);

        public ICommand Update { get; }
        
        private bool ExceedsMaxForStrip(Strip strip)
        {
            var max = LEDs.Counts[strip];
            return ShouldUpdateSingleLED
                ? max < SingleLED
                : max < StopLED;
        }

        private void Update_Execute(object parameter)
        {
            _manualManager.Update();
        }
    }
}
