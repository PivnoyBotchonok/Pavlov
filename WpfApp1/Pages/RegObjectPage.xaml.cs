using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegObjectPage.xaml
    /// </summary>
    public partial class RegObjectPage : Page
    {
        private Flat _flat = new Flat(); // Объект квартиры
        private House _house = new House(); // Объект дома
        private AppData.Region _region = new AppData.Region(); // Объект участка
        private bool _isEdit = false; // Флаг, указывающий, редактируется ли объект

        /// <summary>
        /// Конструктор страницы регистрации объекта.
        /// </summary>
        /// <param name="flat">Объект квартиры, если редактируется существующая квартира.</param>
        /// <param name="house">Объект дома, если редактируется существующий дом.</param>
        /// <param name="reg">Объект участка, если редактируется существующий участок.</param>
        public RegObjectPage(Flat flat = null, House house = null, AppData.Region reg = null)
        {
            InitializeComponent();

            // Заполнение ComboBox владельцами из базы данных
            Owner_cmbBox.ItemsSource = PavlovEntities.GetContext().Client.ToList();
            Owner_cmbBox.DisplayMemberPath = "FIO"; // Указываем, какое свойство отображать
            Owner_cmbBox.SelectedValuePath = "ID"; // Указываем, какое свойство использовать как значение

            if (flat != null) // Если передана квартира для редактирования
            {
                _flat = flat;
                DataContext = _flat; // Установка контекста данных
                FlatCheckData(); // Заполнение полей данными квартиры
                Object_cmbBox.SelectedIndex = 0; // Выбор типа объекта "Квартира"
                Flat.Visibility = Visibility.Visible; // Показ панели квартиры
                _isEdit = true; // Установка флага редактирования
                addBut.Content = "Редактировать"; // Изменение текста кнопки
                Object_cmbBox.IsEnabled = false; // Блокировка ComboBox
            }
            else if (house != null) // Если передан дом для редактирования
            {
                _house = house;
                DataContext = _house;
                HouseCheckData(); // Заполнение полей данными дома
                Object_cmbBox.SelectedIndex = 1; // Выбор типа объекта "Дом"
                House.Visibility = Visibility.Visible;
                _isEdit = true;
                addBut.Content = "Редактировать";
                Object_cmbBox.IsEnabled = false;
            }
            else if (reg != null) // Если передан участок для редактирования
            {
                _region = reg;
                DataContext = _region;
                RegionCheckData(); // Заполнение полей данными участка
                Object_cmbBox.SelectedIndex = 2; // Выбор типа объекта "Участок"
                Region.Visibility = Visibility.Visible;
                _isEdit = true;
                addBut.Content = "Редактировать";
                Object_cmbBox.IsEnabled = false;
            }
        }

        /// <summary>
        /// Скрывает все панели (квартира, дом, участок).
        /// </summary>
        private void ClearPanel()
        {
            Region.Visibility = Visibility.Collapsed;
            House.Visibility = Visibility.Collapsed;
            Flat.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Добавить" или "Редактировать".
        /// Валидирует данные и сохраняет объект в базу данных.
        /// </summary>
        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(Owner_cmbBox.Text))
                {
                    MessageBox.Show("Выберите владельца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Country.Text))
                {
                    MessageBox.Show("Введите город.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Street.Text))
                {
                    MessageBox.Show("Введите улицу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NumHome.Text))
                {
                    MessageBox.Show("Введите номер дома.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка корректности широты и долготы
                if (!double.TryParse(Width.Text, out double latitude) || latitude < -90 || latitude > 90)
                {
                    MessageBox.Show("Широта должна быть числом от -90 до +90.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(Lenght.Text, out double longitude) || longitude < -180 || longitude > 180)
                {
                    MessageBox.Show("Долгота должна быть числом от -180 до +180.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка полей в зависимости от выбранного типа объекта
                if (Object_cmbBox.SelectedIndex == 0) // Квартира
                {
                    if (string.IsNullOrWhiteSpace(Floor.Text) ||
                        string.IsNullOrWhiteSpace(NumRooms_Flat.Text) ||
                        string.IsNullOrWhiteSpace(Area_Flat.Text))
                    {
                        MessageBox.Show("Заполните все поля для квартиры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (_isEdit)
                    {
                        EditFlat(); // Редактирование квартиры
                    }
                    else
                    {
                        AddFlat(); // Добавление квартиры
                    }
                }
                else if (Object_cmbBox.SelectedIndex == 1) // Дом
                {
                    if (string.IsNullOrWhiteSpace(FloorCount.Text) ||
                        string.IsNullOrWhiteSpace(NumRooms_House.Text) ||
                        string.IsNullOrWhiteSpace(Area_House.Text))
                    {
                        MessageBox.Show("Заполните все поля для дома.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (_isEdit)
                    {
                        EditHouse(); // Редактирование дома
                    }
                    else
                    {
                        AddHouse(); // Добавление дома
                    }
                }
                else if (Object_cmbBox.SelectedIndex == 2) // Участок
                {
                    if (string.IsNullOrWhiteSpace(Area_Region.Text))
                    {
                        MessageBox.Show("Заполните площадь участка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (_isEdit)
                    {
                        EditRegion(); // Редактирование участка
                    }
                    else
                    {
                        AddRegion(); // Добавление участка
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Добавляет новую квартиру в базу данных.
        /// </summary>
        private void AddFlat()
        {
            var context = PavlovEntities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: контекст базы данных не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создание объекта квартиры
            Flat flat = new Flat
            {
                ClientID = Convert.ToInt32(Owner_cmbBox.SelectedValue),
                Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text}",
                Coordinate = $"{Width.Text};{Lenght.Text}",
                Floor = Convert.ToInt32(Floor.Text),
                NumRooms = Convert.ToInt32(NumRooms_Flat.Text),
                Area = Convert.ToInt32(Area_Flat.Text)
            };
            context.Flat.Add(flat);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                frameMain.frame.GoBack(); // Возврат на предыдущую страницу
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Редактирует существующую квартиру в базе данных.
        /// </summary>
        private void EditFlat()
        {
            var context = PavlovEntities.GetContext();

            // Обновление данных квартиры
            _flat.ClientID = (int)Owner_cmbBox.SelectedValue;
            _flat.Floor = Convert.ToInt32(Floor.Text);
            _flat.Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text}";
            _flat.Coordinate = $"{Width.Text};{Lenght.Text}";
            _flat.NumRooms = Convert.ToInt32(NumRooms_Flat.Text);
            _flat.Area = Convert.ToInt32(Area_Flat.Text);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Редактирование прошло успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Заполняет поля данными квартиры.
        /// </summary>
        private void FlatCheckData()
        {
            Owner_cmbBox.SelectedValue = _flat.ClientID;
            string input = _flat.Addres;
            string coords = _flat.Coordinate;

            // Используем регулярные выражения для извлечения данных
            Match match = Regex.Match(input, @"г\.([^,]+),\s*ул\.([^,]+),\s*номер дома:(\d+),\s*номер квартиры:(\d+)");
            MatchCollection matches = Regex.Matches(coords, @"-?\d+");

            if (match.Success)
            {
                string city = match.Groups[1].Value.Trim();
                string street = match.Groups[2].Value.Trim();
                string houseNumber = match.Groups[3].Value.Trim();
                string apartmentNumber = match.Groups[4].Value.Trim();
                string[] numbersArray = matches
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();
                Country.Text = city;
                Street.Text = street;
                NumHome.Text = houseNumber;
                NumRoom.Text = apartmentNumber;
                Width.Text = numbersArray[0];
                Lenght.Text = numbersArray[1];
            }
        }

        /// <summary>
        /// Заполняет поля данными дома.
        /// </summary>
        private void HouseCheckData()
        {
            Owner_cmbBox.SelectedValue = _house.ClientID;
            string input = _house.Addres;
            string coords = _house.Coordinate;

            // Используем регулярные выражения для извлечения данных
            Match match = Regex.Match(input, @"г\.([^,]+),\s*ул\.([^,]+),\s*номер дома:(\d+),\s*номер квартиры:(\d+)");
            MatchCollection matches = Regex.Matches(coords, @"-?\d+");

            if (match.Success)
            {
                string city = match.Groups[1].Value.Trim();
                string street = match.Groups[2].Value.Trim();
                string houseNumber = match.Groups[3].Value.Trim();
                string apartmentNumber = match.Groups[4].Value.Trim();
                string[] numbersArray = matches
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();
                Country.Text = city;
                Street.Text = street;
                NumHome.Text = houseNumber;
                NumRoom.Text = apartmentNumber;
                Width.Text = numbersArray[0];
                Lenght.Text = numbersArray[1];
            }
        }

        /// <summary>
        /// Заполняет поля данными участка.
        /// </summary>
        private void RegionCheckData()
        {
            Owner_cmbBox.SelectedValue = _region.ClientID;
            string input = _region.Addres;
            string coords = _region.Coordinate;

            // Используем регулярные выражения для извлечения данных
            Match match = Regex.Match(input, @"г\.([^,]+),\s*ул\.([^,]+),\s*номер дома:(\d+),\s*номер квартиры:(\d+)");
            MatchCollection matches = Regex.Matches(coords, @"-?\d+");

            if (match.Success)
            {
                string city = match.Groups[1].Value.Trim();
                string street = match.Groups[2].Value.Trim();
                string houseNumber = match.Groups[3].Value.Trim();
                string apartmentNumber = match.Groups[4].Value.Trim();
                string[] numbersArray = matches
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .ToArray();
                Country.Text = city;
                Street.Text = street;
                NumHome.Text = houseNumber;
                NumRoom.Text = apartmentNumber;
                Width.Text = numbersArray[0];
                Lenght.Text = numbersArray[1];
            }
        }

        /// <summary>
        /// Добавляет новый дом в базу данных.
        /// </summary>
        private void AddHouse()
        {
            var context = PavlovEntities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: контекст базы данных не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создание объекта дома
            House house = new House
            {
                ClientID = Convert.ToInt32(Owner_cmbBox.SelectedValue),
                Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text}",
                Coordinate = $"{Width.Text};{Lenght.Text}",
                FloorCount = Convert.ToInt32(FloorCount.Text),
                NumRoom = Convert.ToInt32(NumRooms_House.Text),
                Area = Convert.ToInt32(Area_House.Text)
            };
            context.House.Add(house);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Редактирует существующий дом в базе данных.
        /// </summary>
        private void EditHouse()
        {
            var context = PavlovEntities.GetContext();

            // Обновление данных дома
            _house.ClientID = (int)Owner_cmbBox.SelectedValue;
            _house.FloorCount = Convert.ToInt32(FloorCount.Text);
            _house.Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text}";
            _house.Coordinate = $"{Width.Text};{Lenght.Text}";
            _house.NumRoom = Convert.ToInt32(NumRooms_House.Text);
            _house.Area = Convert.ToInt32(Area_House.Text);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Редактирование прошло успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Добавляет новый участок в базу данных.
        /// </summary>
        private void AddRegion()
        {
            var context = PavlovEntities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: контекст базы данных не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создание объекта участка
            AppData.Region region = new AppData.Region
            {
                ClientID = Convert.ToInt32(Owner_cmbBox.SelectedValue),
                Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text}",
                Coordinate = $"{Width.Text};{Lenght.Text}",
                Area = Convert.ToInt32(Area_Region.Text)
            };
            context.Region.Add(region);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Редактирует существующий участок в базе данных.
        /// </summary>
        private void EditRegion()
        {
            var context = PavlovEntities.GetContext();

            // Обновление данных участка
            _region.ClientID = (int)Owner_cmbBox.SelectedValue;
            _region.Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text}";
            _region.Coordinate = $"{Width.Text};{Lenght.Text}";
            _region.Area = Convert.ToInt32(Area_Region.Text);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Редактирование прошло успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                frameMain.frame.GoBack();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений в базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик изменения выбранного элемента в ComboBox.
        /// Показывает соответствующую панель в зависимости от выбранного типа объекта.
        /// </summary>
        private void Object_cmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Object_cmbBox.SelectedIndex == 0) // Квартира
            {
                ClearPanel();
                Flat.Visibility = Visibility.Visible;
            }
            else if (Object_cmbBox.SelectedIndex == 1) // Дом
            {
                ClearPanel();
                House.Visibility = Visibility.Visible;
            }
            else if (Object_cmbBox.SelectedIndex == 2) // Участок
            {
                ClearPanel();
                Region.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Назад".
        /// Возвращает пользователя на предыдущую страницу.
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack();
        }

        /// <summary>
        /// Обработчик ввода текста в поле с цифрами.
        /// Разрешает ввод только цифр.
        /// </summary>
        private void Number_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0)) // Проверка, является ли вводимый символ цифрой
            {
                e.Handled = true; // Отменяем ввод, если символ не цифра
            }
        }
    }
}