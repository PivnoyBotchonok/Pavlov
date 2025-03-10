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

        /// <summary>
        /// Обработчик нажатия на кнопку "Вход".
        /// Проверяет введенные логин и пароль и перенаправляет пользователя на соответствующую страницу.
        /// </summary>
        private void EntryBut_Click(object sender, RoutedEventArgs e)
        {
            // Поиск клиента в базе данных по логину и паролю
            var checkClient = PavlovEntities.GetContext().Client.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);
            // Поиск риелтора в базе данных по логину и паролю
            var checkRieltor = PavlovEntities.GetContext().Rieltor.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);

            if (checkClient != null)
            {
                // Очистка текущих данных пользователя
                frameMain.CurrentClient = null;
                frameMain.CurrentRieltor = null;
                // Сохранение текущего клиента
                frameMain.CurrentClient = checkClient;
                // Переход на главную страницу клиента
                frameMain.frame.Navigate(new ClientMainPage());
            }
            else if (checkRieltor != null)
            {
                // Очистка текущих данных пользователя
                frameMain.CurrentClient = null;
                frameMain.CurrentRieltor = null;
                // Сохранение текущего риелтора
                frameMain.CurrentRieltor = checkRieltor;
                // Переход на главную страницу риелтора
                frameMain.frame.Navigate(new RieltorMainPage());
            }
            else
            {
                // Вывод сообщения об ошибке, если логин или пароль неверны
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "QR-код".
        /// Позволяет пользователю выбрать изображение QR-кода, декодировать его и автоматически заполнить поля логина и пароля.
        /// </summary>
        private void QrBut_Click(object sender, RoutedEventArgs e)
        {
            // Открытие диалога выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG Image|*.png", // Фильтр для выбора только PNG изображений
                Title = "Выберите QR-код" // Заголовок диалогового окна
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Загрузка выбранного изображения
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

                        // Поиск клиента в базе данных по логину и паролю
                        var checkClient = PavlovEntities.GetContext().Client.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);
                        // Поиск риелтора в базе данных по логину и паролю
                        var checkRieltor = PavlovEntities.GetContext().Rieltor.FirstOrDefault(x => x.Login == Login.Text && x.Password == Password.Text);

                        if (checkClient != null)
                        {
                            // Сохранение текущего клиента
                            frameMain.CurrentClient = checkClient;
                            // Переход на главную страницу клиента
                            frameMain.frame.Navigate(new ClientMainPage());
                        }
                        else if (checkRieltor != null)
                        {
                            // Сохранение текущего риелтора
                            frameMain.CurrentRieltor = checkRieltor;
                            // Переход на главную страницу риелтора
                            frameMain.frame.Navigate(new RieltorMainPage());
                        }
                        else
                        {
                            // Вывод сообщения об ошибке, если логин или пароль неверны
                            MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        // Вывод сообщения об ошибке, если формат QR-кода неверный
                        MessageBox.Show("Неверный формат QR-кода", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Вывод сообщения об ошибке, если QR-код не удалось декодировать
                    MessageBox.Show("Не удалось декодировать QR-код", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}