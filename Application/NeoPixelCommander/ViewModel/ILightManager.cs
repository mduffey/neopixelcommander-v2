using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoPixelCommander.ViewModel
{
    public interface ILightManager
    {
        string Name { get; }
        /// <summary>
        /// Is a color manager that updates periodically versus a static one (like SingleColorViewModel).
        /// </summary>
        bool IsActive { get; }
        void Start();
        void Stop();
    }
}
