using NeoPixelCommander.Library.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library.ColorManagers
{
    public class ManualManager
    {
        private readonly PackageHandler _packageHandler;
        public ManualManager(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
            SelectedStrips = new List<Strip>();
        }

        public IList<Strip> SelectedStrips { get; }
        public bool UsingSingleLED { get; set; }
        public int FirstLED { get; set; }
        public int LastLED { get; set; }
        public int SingleLED { get; set; }
        public void Update()
        {
            if (SelectedStrips.Any())
            {
                if (UsingSingleLED)
                {
                    foreach (var strip in SelectedStrips)
                    {
                        _packageHandler.SendRange(new SingleMessage(strip, (byte)SingleLED, Color));
                    }
                    return;
                }
                else
                {
                    var messages = new List<SingleMessage>();
                    foreach (var strip in SelectedStrips)
                    {
                        for (int i = FirstLED - 1; i < LastLED; i++)
                        {
                            messages.Add(new SingleMessage(strip, (byte)i, Color));
                        }
                    }
                    _packageHandler.SendRange(messages);
                }
            }
        }
        public Color Color { get; set; }
    }
}
