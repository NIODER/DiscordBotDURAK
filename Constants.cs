using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constants
{
    public static class Constants
    {
        public static object locker = new object();
        public static readonly string logPath = @"E:\log.TXT";
        public static readonly string tokenPath = @"E:\TOKEN.txt";
    }
}
