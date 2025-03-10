using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
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
using ZXing.Common;
using ZXing;
using System.IO;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Страница регистрации пользователей (клиентов и риелторов).
    /// </summary>
    public partial class RegPage : Page
    {
        public Client _client = new Client(); // Объект клиента
        public Rieltor _rieltor = new Rieltor(); // Объект риелтора
        public bool isEdit; // Флаг, указывающий, редактируется ли существующий пользователь

        // Перечисление для ролей пользователей
        public enum UserRole
        {
            Client,
            Rieltor,
        }

        /// <summary>
        /// Конструктор страницы регистрации.
        /// </summary>
        /// <param name="client">Объект клиента, если редактируется существующий клиент.</param>
        /// <param name="rieltor">Объект риелтора, если редактируется существующий риелтор.</param>
        public RegPage(Client client = null, Rieltor rieltor = null)
        {
            InitializeComponent();

            // Заполнение ComboBox ролями из базы данных
            CmbBox.ItemsSource = DBEntities.GetContext().Role.Select(x => x.RoleName).ToList();

            if (client != null)
            {
                // Инициализация данных для редактирования клиента
                InitializeUser(client, UserRole.Client);
            }
            else if (rieltor != null)
            {
                // Инициализация данных для редактирования риелтора
                InitializeUser(rieltor, UserRole.Rieltor);
            }
            else
            {
                // Скрытие панелей, если пользователь не выбран
                RieltorPanel.Visibility = Visibility.Collapsed;
                ClientPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Инициализация данных пользователя в зависимости от роли.
        /// </summary>
        /// <param name="user">Объект пользователя (клиент или риелтор).</param>
        /// <param name="role">Роль пользователя.</param>
        private void InitializeUser(object user, UserRole role)
        {
            if (user is Client client)
            {
                _client = client;
                DataContext = _client; // Установка контекста данных для клиента
                CmbBox.SelectedIndex = (int)UserRole.Client; // Установка выбранной роли
                ClientPanel.Visibility = Visibility.Visible; // Показ панели клиента
                RieltorPanel.Visibility = Visibility.Collapsed; // Скрытие панели риелтора
            }
            else if (user is Rieltor rieltor)
            {
                _rieltor = rieltor;
                DataContext = _rieltor; // Установка контекста данных для риелтора
                CmbBox.SelectedIndex = (int)UserRole.Rieltor; // Установка выбранной роли
                RieltorPanel.Visibility = Visibility.Visible; // Показ панели риелтора
                ClientPanel.Visibility = Visibility.Collapsed; // Скрытие панели клиента
            }

            CmbBox.IsEnabled = false; // Блокировка ComboBox для редактирования
            regBut.Content = "Редактировать"; // Изменение текста кнопки
            isEdit = true; // Установка флага редактирования
        }

        /// <summary>
        /// Обработчик изменения выбранного элемента в ComboBox.
        /// </summary>
        private void CmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbBox.SelectedIndex == 0) // Если выбрана роль "Клиент"
            {
                RieltorPanel.Visibility = Visibility.Collapsed; // Скрываем панель риелтора
                ClientPanel.Visibility = Visibility.Visible;    // Показываем панель клиента
                regBut.Visibility = Visibility.Visible; // Показывает кнопку
            }
            else if (CmbBox.SelectedIndex == 1) // Если выбрана роль "Риелтор"
            {
                ClientPanel.Visibility = Visibility.Collapsed; // Скрываем панель клиента
                RieltorPanel.Visibility = Visibility.Visible; // Показываем панель риелтора
                regBut.Visibility = Visibility.Visible; // Показывает кнопку
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку регистрации.
        /// </summary>
        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполненность общих полей (Фамилия, Имя, Отчество)
            if (string.IsNullOrWhiteSpace(SName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(FName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(PName_TextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля: Фамилия, Имя, Отчество.");
                return;
            }

            // Проверка на заполненность логина и пароля
            if (string.IsNullOrWhiteSpace(Login_TextBox.Text) || string.IsNullOrWhiteSpace(Pass_TextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните логин и пароль.");
                return;
            }

            var context = DBEntities.GetContext();
            // Проверка на уникальность логина
            bool isLoginExists = context.Client.Any(x => x.Login == Login_TextBox.Text) ||
                                 context.Rieltor.Any(x => x.Login == Login_TextBox.Text);

            if (isLoginExists)
            {
                MessageBox.Show("Пользователь с таким логином уже существует. Пожалуйста, выберите другой логин.");
                return;
            }

            if (CmbBox.SelectedIndex == 0) // Если выбрана роль "Клиент"
            {
                // Проверка на заполненность полей, специфичных для клиента (Номер телефона и Email)
                if (string.IsNullOrWhiteSpace(Number_TextBox.Text) ||
                    string.IsNullOrWhiteSpace(Email_TextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля: Номер телефона и Email.");
                    return;
                }

                // Проверка длины номера телефона (должно быть ровно 11 символов)
                if (Number_TextBox.Text.Length != 11)
                {
                    MessageBox.Show("Номер телефона должен содержать ровно 11 символов.");
                    return;
                }

                if (isEdit)
                {
                    EditClient(); // Редактирование клиента
                    Part_TextBox.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AddClient(); // Добавление клиента
                }
            }
            else if (CmbBox.SelectedIndex == 1) // Если выбрана роль "Риелтор"
            {
                // Проверка корректности ввода доли (должно быть целым числом)
                if (!int.TryParse(Part_TextBox.Text, out int part))
                {
                    MessageBox.Show("Пожалуйста, введите корректное значение для поля 'Part'.");
                    return;
                }

                // Проверка на заполненность поля "Доля"
                if (string.IsNullOrWhiteSpace(Part_TextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля: 'Доля'.");
                    return;
                }

                if (isEdit)
                {
                    EditRieltor(); // Редактирование риелтора
                }
                else
                {
                    AddRieltor(); // Добавление риелтора
                }
            }
        }

        /// <summary>
        /// Добавление нового клиента в базу данных.
        /// </summary>
        private void AddClient()
        {
            var context = DBEntities.GetContext();

            // Создание объекта клиента
            Client client = new Client
            {
                RoleID = 1, // Роль "Клиент"
                SName = SName_TextBox.Text, // Фамилия
                FName = FName_TextBox.Text, // Имя
                PName = PName_TextBox.Text, // Отчество
                Number = Number_TextBox.Text, // Номер телефона
                Emaill = Email_TextBox.Text, // Email
                Login = Login_TextBox.Text, // Логин
                Password = Pass_TextBox.Text, // Пароль
            };

            // Добавление клиента в контекст базы данных
            context.Client.Add(client);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно");
                if (MessageBox.Show("Сгенерировать QR-код с логином и паролем?", "QR-код", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    GenerateAndSaveQRCode(Login_TextBox.Text, Pass_TextBox.Text);
                }
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при сохранении изменений в базе данных: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Редактирование существующего клиента.
        /// </summary>
        private void EditClient()
        {
            var context = DBEntities.GetContext();

            // Обновление данных клиента
            _client.SName = SName_TextBox.Text;
            _client.FName = FName_TextBox.Text;
            _client.PName = PName_TextBox.Text;
            _client.Number = Number_TextBox.Text;
            _client.Emaill = Email_TextBox.Text;
            _client.Login = Login_TextBox.Text;
            _client.Password = Pass_TextBox.Text;

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Редактирование прошло успешно");
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при сохранении изменений в базе данных: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Добавление нового риелтора в базу данных.
        /// </summary>
        private void AddRieltor()
        {
            var context = DBEntities.GetContext();

            // Создание объекта риелтора
            Rieltor rieltor = new Rieltor
            {
                RoleID = 2, // Роль "Риелтор"
                SName = SName_TextBox.Text, // Фамилия
                FName = FName_TextBox.Text, // Имя
                PName = PName_TextBox.Text, // Отчество
                Part = int.Parse(Part_TextBox.Text), // Доля
                Login = Login_TextBox.Text, // Логин
                Password = Pass_TextBox.Text, // Пароль
            };

            // Добавление риелтора в контекст базы данных
            context.Rieltor.Add(rieltor);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно");
                if (MessageBox.Show("Сгенерировать QR-код с логином и паролем?", "QR-код", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    GenerateAndSaveQRCode(Login_TextBox.Text, Pass_TextBox.Text);
                }
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при сохранении изменений в базе данных: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Редактирование существующего риелтора.
        /// </summary>
        private void EditRieltor()
        {
            var context = DBEntities.GetContext();

            // Обновление данных риелтора
            _rieltor.SName = SName_TextBox.Text;
            _rieltor.FName = FName_TextBox.Text;
            _rieltor.PName = PName_TextBox.Text;
            _rieltor.Part = int.Parse(Part_TextBox.Text);
            _rieltor.Login = Login_TextBox.Text;
            _rieltor.Password = Pass_TextBox.Text;

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Редактирование прошло успешно");
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Ошибка при сохранении изменений в базе данных: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("InnerException: " + ex.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Генерация и сохранение QR-кода с логином и паролем.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        private void GenerateAndSaveQRCode(string login, string password)
        {
            // Создаем строку для кодирования в QR-код
            string qrText = $"{login}|{password}";

            // Настройки для генерации QR-кода
            var qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 0
                }
            };

            // Указываем кодировку UTF-8 через Hints
            qrWriter.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

            // Генерация QR-кода
            var qrCodeImage = qrWriter.Write(qrText);

            // Преобразование в BitmapImage для отображения в WPF
            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            // Отображение QR-кода в новом окне
            var qrWindow = new Window
            {
                Title = "QR Code",
                Width = 350,
                Height = 400,
                Content = new Image { Source = bitmapImage }
            };
            qrWindow.Show();

            // Сохранение QR-кода на ПК
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Сохранить QR-код"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    qrCodeImage.Save(fileStream, System.Drawing.Imaging.ImageFormat.Png);
                }
                MessageBox.Show("QR-код успешно сохранен.");
            }
        }

        /// <summary>
        /// Обработчик ввода текста в поле с цифрами.
        /// Разрешает ввод только цифр.
        /// </summary>
        private void Number_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка, является ли вводимый символ цифрой
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Отменяем ввод, если символ не цифра
            }
        }

        /// <summary>
        /// Обработчик ввода текста в поле с буквами.
        /// Разрешает ввод только букв.
        /// </summary>
        private void LettersOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, является ли введенный символ буквой
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Zа-яА-Я]+$"))
            {
                // Если символ не буква, отменяем ввод
                e.Handled = true;
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Назад".
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack(); // Возврат на предыдущую страницу
        }
    }
}