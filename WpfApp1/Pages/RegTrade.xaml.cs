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

            // Заполнение ComboBox риелторами из базы данных
            RealtorComboBox.ItemsSource = PavlovEntities.GetContext().Rieltor.ToList();
            RealtorComboBox.DisplayMemberPath = "FIO"; // Указываем, какое свойство отображать
            RealtorComboBox.SelectedValuePath = "ID";  // Указываем, какое свойство использовать для SelectedValue
            RealtorComboBox.SelectedItem = frameMain.CurrentRieltor; // Установка текущего риелтора
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Зарегистрировать сделку".
        /// Валидирует данные и сохраняет сделку в базу данных.
        /// </summary>
        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            // Проверка выбора риелтора
            var selectedRealtor = RealtorComboBox.SelectedItem as Rieltor;
            if (selectedRealtor == null)
            {
                MessageBox.Show("Выберите риелтора", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int rieltorId = selectedRealtor.ID;

            // Проверка выбора объекта недвижимости
            if (PropertyComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите объект недвижимости", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Приведение SelectedValue к int (Id объекта недвижимости)
            int propertyId = (int)PropertyComboBox.SelectedValue;

            // Проверка и преобразование CommissionTextBlock.Text
            string commissionText = CommissionTextBlock.Text.Replace("Отчисление: ", "").Replace("₽", "").Trim();
            if (!decimal.TryParse(commissionText, out decimal commission))
            {
                MessageBox.Show("Некорректное значение отчисления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка даты
            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату сделки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DateTime dateTrade = DatePicker.SelectedDate.Value;

            // Проверка суммы сделки
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Некорректная сумма сделки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Выберите тип недвижимости", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            // Добавление сделки в базу данных
            addTrade(trade);
        }

        /// <summary>
        /// Обработчик изменения выбранного элемента в ComboBox типа недвижимости.
        /// Заполняет ComboBox объектами недвижимости в зависимости от выбранного типа.
        /// </summary>
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (TypeComboBox.SelectedIndex)
            {
                case 0: // Квартира
                    PropertyComboBox.SelectedIndex = -1;
                    PropertyComboBox.ItemsSource = PavlovEntities.GetContext().Flat.ToList(); // Заполняем объектами Flat
                    PropertyComboBox.DisplayMemberPath = "Addres"; // Отображаем адрес
                    PropertyComboBox.SelectedValuePath = "ID";     // Используем Id для SelectedValue
                    break;
                case 1: // Дом
                    PropertyComboBox.SelectedIndex = -1;
                    PropertyComboBox.ItemsSource = PavlovEntities.GetContext().House.ToList(); // Заполняем объектами House
                    PropertyComboBox.DisplayMemberPath = "Addres";
                    PropertyComboBox.SelectedValuePath = "ID";
                    break;
                case 2: // Участок
                    PropertyComboBox.SelectedIndex = -1;
                    PropertyComboBox.ItemsSource = PavlovEntities.GetContext().Region.ToList(); // Заполняем объектами Region
                    PropertyComboBox.DisplayMemberPath = "Addres";
                    PropertyComboBox.SelectedValuePath = "ID";
                    break;
            }
        }

        /// <summary>
        /// Обработчик изменения текста в поле "Сумма сделки".
        /// Рассчитывает отчисление риелтора и обновляет TextBlock.
        /// </summary>
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

        /// <summary>
        /// Добавляет сделку в базу данных.
        /// </summary>
        /// <param name="trade">Объект сделки для добавления.</param>
        private void addTrade(Trade trade)
        {
            var context = PavlovEntities.GetContext();
            context.Trade.Add(trade);
            try
            {
                // Сохранение изменений в базе данных
                context.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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