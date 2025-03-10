using Microsoft.Win32;
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
using ZXing;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Страница входа в систему (логин).
    /// </summary>
    public partial class LogPage : Page
    {
        /// <summary>
        /// Конструктор страницы входа.
        /// </summary>
        public LogPage()
        {
            InitializeComponent(); // Инициализация компонентов страницы
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Регистрация".
        /// Перенаправляет пользователя на страницу регистрации.
        /// </summary>
        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации (RegPage)
            frameMain.frame.Navigate(new RegPage(null));
        }

        private void EntryBut_Click(object sender, RoutedEventArgs e)
        {
            var checkClient = DBEntities.GetContext().Client.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);
            var checkRieltor = DBEntities.GetContext().Rieltor.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);

            if (checkClient != null)
            {
                frameMain.CurrentClient = null;
                frameMain.CurrentRieltor = null;
                frameMain.CurrentClient = checkClient; // Сохраняем клиента
                frameMain.frame.Navigate(new ClientMainPage());
            }
            else if (checkRieltor != null)
            {
                frameMain.CurrentClient = null;
                frameMain.CurrentRieltor = null;
                frameMain.CurrentRieltor = checkRieltor; // Сохраняем риелтора
                frameMain.frame.Navigate(new RieltorMainPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void QrBut_Click(object sender, RoutedEventArgs e)
        {
            // Открытие диалога выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Выберите QR-код"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Загрузка изображения
                var bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));

                // Декодирование QR-кода
                var reader = new BarcodeReader();
                var result = reader.Decode(bitmapImage);

                if (result != null)
                {
                    // Разделение строки по разделителю
                    string[] parts = result.Text.Split('|');

                    if (parts.Length == 2)
                    {
                        string login = parts[0];
                        string password = parts[1];

                        // Заполнение полей логина и пароля
                        Login.Text = login;
                        Password.Text = password;
                        var checkClient = DBEntities.GetContext().Client.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);
                        var checkRieltor = DBEntities.GetContext().Rieltor.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);
                        if (checkClient != null)
                        {
                            frameMain.CurrentClient = checkClient; // Сохраняем клиента
                            frameMain.frame.Navigate(new ClientMainPage());
                        }
                        else if (checkRieltor != null)
                        {
                            frameMain.CurrentRieltor = checkRieltor; // Сохраняем риелтора
                            frameMain.frame.Navigate(new RieltorMainPage());
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат QR-кода", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось декодировать QR-код", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}