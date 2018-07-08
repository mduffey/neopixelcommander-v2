using NeoPixelCommander.Library;
using System.Windows.Media;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public class OffViewModel : ILightManager, IAutomaticLightManager
    {
        private readonly PackageHandler _packageHandler;
        public string Name => "Off";

        public OffViewModel(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
        }

        public void Start()
        {
            _packageHandler.SendUniversal(Colors.Black);
        }
    }
}
