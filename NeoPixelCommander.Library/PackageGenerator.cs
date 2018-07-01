using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NeoPixelCommander.Library
{
    public class PackageGenerator
    {
        private int _strip, _startRange, _endRange;
        private byte _red, _green, _blue;

        protected PackageGenerator(int strip)
        {
            _strip = strip;
        }

        public static PackageGenerator Create(int strip)
        {
            return new PackageGenerator(strip);            
        }

        public PackageGenerator SetRange(int start, int end)
        {
            _startRange = start;
            _endRange = end;
            return this;
        }

        public PackageGenerator SetColor(Color color)
        {
            _red = color.R;
            _green = color.G;
            _blue = color.B;
            return this;
        }

        public byte[][] BuildPackets()
        {
            var result = new byte[CountPacketsRequired()][];
            var currentIndex = _startRange;
            for(var i = 0; i < CountPacketsRequired(); i++)
            {
                var output = new byte[65];
                var finished = false;
                int b = 1;
                while (b < 65 && !finished)
                {
                    output[b] = CreateLocation(currentIndex);
                    output[b + 1] = _red;
                    output[b + 2] = _green;
                    output[b + 3] = _blue;
                    currentIndex++;
                    if (currentIndex > _endRange)
                        finished = true;
                    b += 4;
                }
                if (b < 65)
                {
                    output[b] = 255; // Send max value as an 'end-of-packet'
                }
                result[i] = output;
            }
            return result;
        }

        protected byte CreateLocation(int index)
        {
            int output = index << 2;
            output = output | _strip;
            return (byte)output;
        }
        
        protected int CountPacketsRequired()
        {
            var totalByes = (_endRange - _startRange + 1) * 4;
            var packets = totalByes / 64;
            if (totalByes % 64 > 0)
                packets++;
            return packets;
        }
    }
}
