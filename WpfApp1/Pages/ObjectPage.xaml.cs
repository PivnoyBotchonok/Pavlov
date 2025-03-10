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
            InitializeComponent();
        }
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack();
        }
        private void TradeBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new TradePage());
        }
        private void Object_cmbBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Object_cmbBox.SelectedIndex == 0)
            {
                frameTable.Navigate(new FlatTable());

            }
            else if (Object_cmbBox.SelectedIndex == 1)
            {
                frameTable.Navigate(new HouseTable());
            }
            else if (Object_cmbBox.SelectedIndex == 2)
            {
                frameTable.Navigate(new RegionTable());
            }
        }

        private void RieltorBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RieltorMainPage());
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Перезагружаем данные из базы данных
                var context = DBEntities.GetContext();
                context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                frameTable.Refresh();
            }
        }
    }
}
