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
        void Start();
        void Stop();
    }
}
