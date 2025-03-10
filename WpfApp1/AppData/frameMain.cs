using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1.AppData
{
    /// <summary>
    /// Класс для управления навигацией и хранения текущего состояния приложения.
    /// </summary>
    class frameMain
    {
        /// <summary>
        /// Статическое свойство для хранения ссылки на Frame, используемый для навигации между страницами.
        /// </summary>
        public static Frame frame { get; set; }

        /// <summary>
        /// Статическое свойство, указывающее, работает ли приложение в режиме клиента.
        /// </summary>
        public static bool isClient { get; set; }

        /// <summary>
        /// Статическое свойство, указывающее, работает ли приложение в режиме риелтора.
        /// </summary>
        public static bool isRieltor { get; set; }

        /// <summary>
        /// Статическое свойство для хранения текущего клиента, вошедшего в систему.
        /// </summary>
        public static Client CurrentClient { get; set; }

        /// <summary>
        /// Статическое свойство для хранения текущего риелтора, вошедшего в систему.
        /// </summary>
        public static Rieltor CurrentRieltor { get; set; }
    }
}