using System;
using System.Collections.Generic;
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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegTrade.xaml
    /// </summary>
    public partial class RegTrade : Page
    {
        public RegTrade()
        {
            InitializeComponent();
            RealtorComboBox.ItemsSource = DBEntities.GetContext().Rieltor.ToList();
            RealtorComboBox.DisplayMemberPath = "FIO"; // Указываем, какое свойство отображать
            RealtorComboBox.SelectedValuePath = "ID";  // Указываем, какое свойство использовать для SelectedValue
            RealtorComboBox.SelectedItem = frameMain.CurrentRieltor;
        }

        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            // Проверка выбора риелтора
            var selectedRealtor = RealtorComboBox.SelectedItem as Rieltor;
            if (selectedRealtor == null)
            {
                MessageBox.Show("Выберите риелтора");
                return;
            }
            int rieltorId = selectedRealtor.ID;
            // Проверка выбора объекта недвижимости
            if (PropertyComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите объект недвижимости");
                return;
            }

            // Приведение SelectedValue к int (Id объекта недвижимости)
            int propertyId = (int)PropertyComboBox.SelectedValue;

            // Проверка и преобразование CommissionTextBlock.Text
            string commissionText = CommissionTextBlock.Text.Replace("Отчисление: ", "").Replace("₽", "").Trim();
            if (!decimal.TryParse(commissionText, out decimal commission))
            {
                MessageBox.Show("Некорректное значение отчисления");
                return;
            }

            // Проверка даты
            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату сделки");
                return;
            }
            DateTime dateTrade = DatePicker.SelectedDate.Value;

            // Проверка суммы сделки
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Некорректная сумма сделки");
                return;
            }

            // Создание объекта Trade в зависимости от выбранного типа недвижимости
            Trade trade = new Trade
            {
                RieltorID = rieltorId,
                DateTrade = dateTrade,
                Amount = amount,
                RieltorPart = commission
            };

            switch (TypeComboBox.SelectedIndex)
            {
                case 0: // Квартира
                    trade.FlatID = propertyId;
                    trade.HouseID = null;
                    trade.RegionID = null;
                    break;
                case 1: // Дом
                    trade.FlatID = null;
                    trade.HouseID = propertyId;
                    trade.RegionID = null;
                    break;
                case 2: // Участок
                    trade.FlatID = null;
                    trade.HouseID = null;
                    trade.RegionID = propertyId;
                    break;
                default:
                    MessageBox.Show("Выберите тип недвижимости");
                    return;
            }

            // Добавление сделки в базу данных
            addTrade(trade);
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TypeComboBox.SelectedIndex)
            {
                case 0: // Квартира
                    PropertyComboBox.SelectedIndex = -1;
                    PropertyComboBox.ItemsSource = DBEntities.GetContext().Flat.ToList(); // Заполняем объектами Flat
                    PropertyComboBox.DisplayMemberPath = "Addres"; // Отображаем адрес
                    PropertyComboBox.SelectedValuePath = "ID";     // Используем Id для SelectedValue
                    
                    break;
                case 1: // Дом
                    PropertyComboBox.SelectedIndex = -1;
                    PropertyComboBox.ItemsSource = DBEntities.GetContext().House.ToList(); // Заполняем объектами House
                    PropertyComboBox.DisplayMemberPath = "Addres";
                    PropertyComboBox.SelectedValuePath = "ID";
                    break;
                case 2: // Участок
                    PropertyComboBox.SelectedIndex = -1;
                    PropertyComboBox.ItemsSource = DBEntities.GetContext().Region.ToList(); // Заполняем объектами Region
                    PropertyComboBox.DisplayMemberPath = "Addres";
                    PropertyComboBox.SelectedValuePath = "ID";
                    break;
            }

        }
        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedRealtor = RealtorComboBox.SelectedItem as Rieltor;

            // Проверяем, выбран ли риелтор
            if (selectedRealtor == null)
            {
                CommissionTextBlock.Text = "Выберите риелтора";
                return;
            }

            // Получаем текст из TextBox
            string amountText = AmountTextBox.Text;

            // Проверяем, является ли введенное значение числом
            if (decimal.TryParse(amountText, out decimal amount))
            {
                // Получаем процент отчисления (Part) из выбранного риелтора
                int part = selectedRealtor.Part;

                // Рассчитываем отчисление
                decimal commission = amount * (part / 100m); // Делим на 100, чтобы получить процент

                // Обновляем TextBlock с информацией об отчислении
                CommissionTextBlock.Text = $"Отчисление: {commission:C}"; // Форматируем как валюту
            }
            else
            {
                // Если введенное значение не является числом, очищаем TextBlock
                CommissionTextBlock.Text = "Отчисление: Некорректная сумма";
            }
        }
        private void addTrade(Trade trade) 
        {
            var context = DBEntities.GetContext();
            context.Trade.Add(trade);
            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно");
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
