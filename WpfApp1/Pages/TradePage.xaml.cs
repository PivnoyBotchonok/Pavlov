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
    /// Логика взаимодействия для TradePage.xaml
    /// </summary>
    public partial class TradePage : Page
    {
        public TradePage()
        {
            InitializeComponent();
            dataGrid.ItemsSource = DBEntities.GetContext().Trade.ToList();
        }

        private void RieltorBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RieltorMainPage());
        }

        private void ObjectBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new ObjectPage());
        }

        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.GoBack();
        }

        private void addBut_Click(object sender, RoutedEventArgs e)
        {
            frameMain.frame.Navigate(new RegTrade());
        }
    }
}
