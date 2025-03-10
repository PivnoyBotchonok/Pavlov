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
    /// Логика взаимодействия для ClientMainPage.xaml
    /// </summary>
    public partial class ClientMainPage : Page
    {
        public ClientMainPage()
        {
            InitializeComponent();
            dataGrid.ItemsSource = DBEntities.GetContext().Client.ToList();
        }

        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.isClient = true;
            frameMain.frame.Navigate(new RegPage(null));
        }

        private void delBut_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранные элементы для удаления из DataGrid
            var itemsForRemoving = dataGrid.SelectedItems.Cast<Client>().ToList();

            // Проверяем, есть ли выбранные элементы
            if (itemsForRemoving.Count == 0)
            {
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
                    DBEntities.GetContext().Client.RemoveRange(itemsForRemoving);

                    // Сохраняем изменения в базе данных
                    DBEntities.GetContext().SaveChanges();

                    // Уведомляем пользователя об успешном удалении
                    MessageBox.Show("Данные удалены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновляем источник данных для DataGrid
                    dataGrid.ItemsSource = DBEntities.GetContext().Client.ToList();
                }
                catch (Exception ex)
                {
                    // Обрабатываем исключение и выводим сообщение об ошибке
                    MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void editBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RegPage((sender as Button).DataContext as Client));
        }
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new LogPage());
        }
    }
}
