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

namespace WpfApp1.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для RegionTable.xaml
    /// </summary>
    public partial class RegionTable : Page
    {
        public RegionTable()
        {
            InitializeComponent();

            // Загрузка данных об участках в DataGrid
            dataGrid.ItemsSource = PavlovEntities.GetContext().Region.ToList();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Добавить".
        /// Переход на страницу регистрации нового участка.
        /// </summary>
        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RegObjectPage(null)); // Переход на страницу регистрации
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Удалить".
        /// Удаление выбранных участков из базы данных.
        /// </summary>
        private void delBut_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранные элементы для удаления из DataGrid
            var itemsForRemoving = dataGrid.SelectedItems.Cast<Region>().ToList();

            // Проверяем, есть ли выбранные элементы
            if (itemsForRemoving.Count == 0)
            {
                // Вывод сообщения, если не выбрано ни одного элемента
                MessageBox.Show("Не выбрано ни одного элемента для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Запрашиваем подтверждение удаления
            if (MessageBox.Show($"Вы точно хотите удалить следующие {itemsForRemoving.Count} элементов?", "Внимание",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем выбранные элементы из контекста базы данных
                    PavlovEntities.GetContext().Region.RemoveRange(itemsForRemoving);

                    // Сохраняем изменения в базе данных
                    PavlovEntities.GetContext().SaveChanges();

                    // Уведомляем пользователя об успешном удалении
                    MessageBox.Show("Данные удалены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем источник данных для DataGrid
                    dataGrid.ItemsSource = PavlovEntities.GetContext().Region.ToList();
                }
                catch (Exception ex)
                {
                    // Обрабатываем исключение и выводим сообщение об ошибке
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Редактировать".
        /// Переход на страницу редактирования выбранного участка.
        /// </summary>
        private void editBut_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу редактирования с передачей выбранного участка
            frameMain.frame.Navigate(new RegObjectPage(null, null, (sender as Button).DataContext as Region));
        }

        /// <summary>
        /// Обработчик изменения видимости страницы.
        /// При отображении страницы обновляет данные в DataGrid.
        /// </summary>
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible) // Если страница стала видимой
            {
                var context = PavlovEntities.GetContext();
                context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload()); // Перезагружаем данные
                dataGrid.ItemsSource = context.Region.ToList(); // Обновляем источник данных для DataGrid
            }
        }
    }
}