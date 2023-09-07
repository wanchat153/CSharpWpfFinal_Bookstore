using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSharpWpfFinal_Bookstore
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {

        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void emailText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void passwordText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Bookstore bookstore = new Bookstore();
            bookstore.Show();
            this.Close();
        }
    }
}
