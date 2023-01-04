using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphical.Utils
{
    internal class FileHeaders
    {
        public static Dictionary<string, string> Headers = new Dictionary<string, string>()
        {
            { "504B0304", "zip" },
            { "FFD8FFE0", "jpg" }
        };
    }
}
