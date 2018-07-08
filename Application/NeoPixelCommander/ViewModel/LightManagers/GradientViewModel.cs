using GalaSoft.MvvmLight;
using NeoPixelCommander.Library;
using NeoPixelCommander.Library.ColorManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class GradientViewModel: ViewModelBase, ILightManager, IAutomaticLightManager
    {
        private readonly GradientManager _gradientManager;
        public string Name => "Gradient";

        public GradientViewModel(GradientManager gradientManager)
        {
            _gradientManager = gradientManager;
            _gradientManager.FirstColor = Colors.Yellow;
            _gradientManager.SecondColor = Colors.Purple;
        }

        public Color StartColor
        {
            get => _gradientManager.FirstColor;
            set
            {
                _gradientManager.FirstColor = value;
                Start();
                RaisePropertyChanged();
            }
        }

        public Color StopColor
        {
            get => _gradientManager.SecondColor;
            set
            {
                _gradientManager.SecondColor = value;
                Start();
                RaisePropertyChanged();
            }
        }

        public void Start()
        {
            _gradientManager.Send();
        }
    }
}
