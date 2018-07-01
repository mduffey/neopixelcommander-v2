using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorControl
{
    public static class IntExtension
    {
        public static int ToInt(this string text)
        {
            if (int.TryParse(text, out var val))
                return val;
            return 0;
        }
    }
}
