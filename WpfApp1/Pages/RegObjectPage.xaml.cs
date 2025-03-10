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
    /// Логика взаимодействия для ObjectPage.xaml
    /// </summary>
    public partial class RegObjectPage : Page
    {
        private Flat _flat = new Flat();
        private House _house = new House();
        private AppData.Region _region = new AppData.Region();
        private bool _isEdit = false;
        public RegObjectPage(Flat flat = null, House house = null, AppData.Region reg = null)
        {
            InitializeComponent();
            Owner_cmbBox.ItemsSource = DBEntities.GetContext().Client.ToList();
            Owner_cmbBox.DisplayMemberPath = "FIO"; // Указываем, какое свойство отображать
            Owner_cmbBox.SelectedValuePath = "ID"; // Указываем, какое свойство использовать как значение
            if (flat != null)
            {
                _flat = flat;
                DataContext = _flat;
                FlatCheckData();
                Object_cmbBox.SelectedIndex = 0;
                Flat.Visibility = Visibility.Visible;
                _isEdit = true;
                addBut.Content = "Редактировать";
                Object_cmbBox.IsEnabled = false;
            }
            else if (house != null)
            {
                _house = house;
                DataContext = _house;
                HouseCheckData();
                Object_cmbBox.SelectedIndex = 1;
                House.Visibility = Visibility.Visible;
                _isEdit = true;
                addBut.Content = "Редактировать";
                Object_cmbBox.IsEnabled = false;
            }
            else if (reg != null)
            {
                _region = reg;
                DataContext = _region;
                RegionCheckData();
                Object_cmbBox.SelectedIndex = 2;
                Region.Visibility = Visibility.Visible;
                _isEdit = true;
                addBut.Content = "Редактировать";
                Object_cmbBox.IsEnabled = false;
            }
        }
        private void ClearPanel() 
        {
            Region.Visibility = Visibility.Collapsed;
            House.Visibility = Visibility.Collapsed;
            Flat.Visibility = Visibility.Collapsed;
        }

        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Owner_cmbBox.Text))
                {
                    MessageBox.Show("Выберите владельца.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Country.Text))
                {
                    MessageBox.Show("Введите город.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Street.Text))
                {
                    MessageBox.Show("Введите улицу.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NumHome.Text))
                {
                    MessageBox.Show("Введите номер дома.");
                    return;
                }

                if (!double.TryParse(Width.Text, out double latitude) || latitude < -90 || latitude > 90)
                {
                    MessageBox.Show("Широта должна быть числом от -90 до +90.");
                    return;
                }

                if (!double.TryParse(Lenght.Text, out double longitude) || longitude < -180 || longitude > 180)
                {
                    MessageBox.Show("Долгота должна быть числом от -180 до +180.");
                    return;
                }

                // Проверка полей в зависимости от выбранного типа объекта
                if (Object_cmbBox.SelectedIndex == 0) // Квартира
                {
                    if (string.IsNullOrWhiteSpace(Floor.Text) ||
                        string.IsNullOrWhiteSpace(NumRooms_Flat.Text) ||
                        string.IsNullOrWhiteSpace(Area_Flat.Text))
                    {
                        MessageBox.Show("Заполните все поля для квартиры.");
                        return;
                    }
                    if (_isEdit)
                    {
                        EditFlat();
                    }
                    else
                    {
                        AddFlat();
                    }
                }
                else if (Object_cmbBox.SelectedIndex == 1) // Дом
                {
                    if (string.IsNullOrWhiteSpace(FloorCount.Text) ||
                        string.IsNullOrWhiteSpace(NumRooms_House.Text) ||
                        string.IsNullOrWhiteSpace(Area_House.Text))
                    {
                        MessageBox.Show("Заполните все поля для дома.");
                        return;
                    }
                    if (_isEdit)
                    {
                        EditHouse();
                    }
                    else
                    {
                        AddHouse();
                    }
                }
                else if (Object_cmbBox.SelectedIndex == 2) // Участок
                {
                    if (string.IsNullOrWhiteSpace(Area_Region.Text))
                    {
                        MessageBox.Show("Заполните площадь участка.");
                        return;
                    }
                    if (_isEdit)
                    {
                        EditRegion();
                    }
                    else
                    {
                        AddRegion();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
        private void AddFlat()
        {
            var context = DBEntities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: контекст базы данных не инициализирован.");
                return;
            }

            // Создание объекта клиента
            Flat flat = new Flat
            {
                ClientID = Convert.ToInt32(Owner_cmbBox.SelectedValue),
                Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text} ",
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
                MessageBox.Show("Регистрация прошло успешно");
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
        private void EditFlat() 
        {
            var context = DBEntities.GetContext();

            // Обновление данных квартиры
            _flat.ClientID = (int)Owner_cmbBox.SelectedValue;
            _flat.Floor = Convert.ToInt32(Floor.Text);
            _flat.Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text} ";
            _flat.Coordinate = $"{Width.Text};{Lenght.Text}";
            _flat.NumRooms = Convert.ToInt32(NumRooms_Flat.Text);
            _flat.Area = Convert.ToInt32(Area_Flat.Text);

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

        private void AddHouse() 
        {
            var context = DBEntities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: контекст базы данных не инициализирован.");
                return;
            }

            // Создание объекта клиента
            House house = new House
            {
                ClientID = Convert.ToInt32(Owner_cmbBox.SelectedValue),
                Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text} ",
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
                MessageBox.Show("Регистрация прошло успешно");
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
        private void EditHouse() 
        {
            var context = DBEntities.GetContext();

            // Обновление данных квартиры
            _house.ClientID = (int)Owner_cmbBox.SelectedValue;
            _house.FloorCount = Convert.ToInt32(Floor.Text);
            _house.Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text} ";
            _house.Coordinate = $"{Width.Text};{Lenght.Text}";
            _house.NumRoom = Convert.ToInt32(NumRooms_House.Text);
            _house.NumRoom = Convert.ToInt32(Area_House.Text);
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

        private void AddRegion()
        {
            var context = DBEntities.GetContext();
            if (context == null)
            {
                MessageBox.Show("Ошибка: контекст базы данных не инициализирован.");
                return;
            }

            // Создание объекта клиента
            AppData.Region region = new AppData.Region
            {
                ClientID = Convert.ToInt32(Owner_cmbBox.SelectedValue),
                Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text} ",
                Coordinate = $"{Width.Text};{Lenght.Text}",
                Area = Convert.ToInt32(Area_Region.Text)
            };
            context.Region.Add(region);

            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошло успешно");
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
        private void EditRegion()
        {
            var context = DBEntities.GetContext();

            // Обновление данных квартиры
            _region.ClientID = (int)Owner_cmbBox.SelectedValue;
            _region.Addres = $"г.{Country.Text}, ул.{Street.Text}, номер дома:{NumHome.Text}, номер квартиры:{NumRoom.Text} ";
            _region.Coordinate = $"{Width.Text};{Lenght.Text}";
            _region.Area = Convert.ToInt32(Area_Region.Text);
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
        private void Object_cmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Object_cmbBox.SelectedIndex == 0)
            {
                ClearPanel();
                Flat.Visibility = Visibility.Visible;
            }
            else if (Object_cmbBox.SelectedIndex == 1)
            {
                ClearPanel();
                House.Visibility = Visibility.Visible;
            }
            else if (Object_cmbBox.SelectedIndex == 2)
            {
                ClearPanel();
                Region.Visibility = Visibility.Visible;
            }
        }

        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack();
        }
        private void Number_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка, является ли вводимый символ цифрой
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Отменяем ввод, если символ не цифра
            }
        }
    }
}
