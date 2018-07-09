using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.ViewModel.LightManagers
{
    public interface ILightManager
    {
        string Name { get; }
    }

    /// <summary>
    /// A light manager that will update the LEDs automatically whenever a change is made.
    /// <para>Implements Start so that the main view model can automatically set the LEDs to the current state of the view model when it's selected.</para>
    /// </summary>
    public interface IAutomaticLightManager : ILightManager
    {
        void Start();
    }

    /// <summary>
    /// An automatic light manager that actively manages the LEDs (periodically changes their colors).
    /// </summary>
    public interface IActiveLightManager : ILightManager
    {
        void Start();
        void Stop();
    }
}
