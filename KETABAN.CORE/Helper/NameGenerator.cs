using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KETABAN.CORE.Helper
{
    public class NameGenerator
    {
        public static string nameGenerator()
        {

            return Guid.NewGuid().ToString().Replace("_", "");

        }
    }
}
