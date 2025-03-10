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
using WpfApp1.Pages.Tables;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectPage.xaml
    /// </summary>
    public partial class ObjectPage : Page
    {
        public ObjectPage()
        {
            InitializeComponent(); // Инициализация компонентов страницы
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Назад".
        /// Возвращает пользователя на предыдущую страницу.
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack(); // Переход на предыдущую страницу
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Торговля".
        /// Переход на страницу торговли.
        /// </summary>
        private void TradeBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new TradePage()); // Переход на страницу TradePage
        }

        /// <summary>
        /// Обработчик изменения выбранного элемента в ComboBox.
        /// В зависимости от выбранного элемента загружает соответствующую таблицу.
        /// </summary>
        private void Object_cmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Object_cmbBox.SelectedIndex == 0) // Если выбран "Квартиры"
            {
                frameTable.Navigate(new FlatTable()); // Загрузка таблицы квартир
            }
            else if (Object_cmbBox.SelectedIndex == 1) // Если выбран "Дома"
            {
                frameTable.Navigate(new HouseTable()); // Загрузка таблицы домов
            }
            else if (Object_cmbBox.SelectedIndex == 2) // Если выбран "Регионы"
            {
                frameTable.Navigate(new RegionTable()); // Загрузка таблицы регионов
            }
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
        /// Обработчик изменения видимости страницы.
        /// При отображении страницы обновляет данные в таблице.
        /// </summary>
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible) // Если страница стала видимой
            {
                // Перезагружаем данные из базы данных
                var context = PavlovEntities.GetContext();
                context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());

                // Обновляем содержимое фрейма с таблицей
                frameTable.Refresh();
            }
        }
    }
}