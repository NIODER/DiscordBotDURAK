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
        public static readonly string tabulation = "\n__________________________________________\n";
    }

    public static class Commands
    {
        public static readonly string random = "$random"; 
        public static readonly string spam = "$spam"; 
        public static readonly string moderate = "$moderate"; 
        public static readonly string clean = "$clean"; 
        public static readonly string decide = "реши"; 
        public static readonly string id = "$ID"; 
    }
}
