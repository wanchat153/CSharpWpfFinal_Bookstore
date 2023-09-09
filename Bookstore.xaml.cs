using CSharpWpfFinal_Bookstore.data;
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
using System.Windows.Shapes;

namespace CSharpWpfFinal_Bookstore
{
    /// <summary>
    /// Interaction logic for Bookstore.xaml
    /// </summary>
    public partial class Bookstore : Window
    {
        public Bookstore()
        {
            InitializeComponent();
            DataCustomers.InitializeDataCustomers();
            DataBookstore.InitializeDataBookstore();
            DataOders.InitializeDataOders();
        }

        private void customersBtn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.NavigationService.Navigate(new CustomersPage());
        }

        private void booksBtn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.NavigationService.Navigate(new BooksPage());
        }

        private void ordersBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
