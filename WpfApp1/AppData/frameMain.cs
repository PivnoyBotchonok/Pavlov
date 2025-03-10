using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1.AppData
{
    class frameMain
    {
        public static Frame frame { get; set; }
        public static bool isClient { get; set; }
        public static bool isRieltor { get; set; }
        public static Client CurrentClient { get; set; }
        public static Rieltor CurrentRieltor { get; set; }
    }
}
