using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.AppData;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для TradePage.xaml
    /// </summary>
    public partial class TradePage : Page
    {
        public TradePage()
        {
            InitializeComponent();

            // Загрузка данных о сделках в DataGrid
            dataGrid.ItemsSource = PavlovEntities.GetContext().Trade.ToList();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Риелторы".
        /// Переход на главную страницу риелторов.
        /// </summary>
        private void RieltorBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RieltorMainPage()); // Переход на страницу RieltorMainPage
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Объекты".
        /// Переход на страницу объектов недвижимости.
        /// </summary>
        private void ObjectBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new ObjectPage()); // Переход на страницу ObjectPage
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Назад".
        /// Возврат на предыдущую страницу.
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack(); // Возврат на предыдущую страницу
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Добавить".
        /// Переход на страницу регистрации новой сделки.
        /// </summary>
        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RegTrade()); // Переход на страницу RegTrade
        }
    }
}