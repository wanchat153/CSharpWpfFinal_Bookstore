using CSharpWpfFinal_Bookstore.data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace CSharpWpfFinal_Bookstore
{
    /// <summary>
    /// Interaction logic for CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            InitializeComponent();
            DataCustomers.InitializeDataCustomers();
            DataBookstore.InitializeDataBookstore();
            DataOders.InitializeDataOders();
            RefreshData();
        }

        public void RefreshData()
        {
            string sql = "SELECT * FROM Customers";

            using (SqliteConnection conn = new SqliteConnection("Data Source=Bookstore.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        DataGridCustomers.ItemsSource = dt.DefaultView;
                    }
                }
                conn.Close();
            }
        }

        private void searchCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchCustomers.Text.Trim();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string sql;
                if (int.TryParse(searchText, out int customerId))
                {
                    // แปลง customerId เป็นข้อความ
                    string customerIdText = customerId.ToString();
                    sql = $"SELECT * FROM Customers WHERE Customer_id = '{customerIdText}'";
                }
                else
                {
                    sql = "SELECT * FROM Customers WHERE Customer_Name LIKE @SearchText";
                }

                using (SqliteConnection conn = new SqliteConnection("Data Source=Bookstore.db"))
                {
                    conn.Open();

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        // ไม่ต้องใช้ parameter สำหรับ customerId
                        if (!int.TryParse(searchText, out int _))
                        {
                            cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                        }

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            DataGridCustomers.ItemsSource = dt.DefaultView;
                        }
                    }
                    conn.Close();
                }
            }
            else
            {
                // ถ้าไม่มีข้อความค้นหา ให้แสดงข้อมูลทั้งหมด
                RefreshData();
            }
        }

        private void idCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void nameCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addressCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void emailCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addCustomers_Click(object sender, RoutedEventArgs e)
        {
            // รับข้อมูลจาก TextBoxes ในหน้า UI
            int IdCustomers;
            string NameCustomers = nameCustomers.Text;
            string AddressCustomers = addressCustomers.Text;
            string EmailCustomers = emailCustomers.Text;

            if (int.TryParse(idCustomers.Text, out IdCustomers))
            {
                if (!string.IsNullOrWhiteSpace(NameCustomers) && !string.IsNullOrWhiteSpace(AddressCustomers) && !string.IsNullOrWhiteSpace(EmailCustomers))
                {
                    if (!IsCustomerExists(IdCustomers, NameCustomers))
                    {
                        // เรียกใช้เมธอด AddDataCustomers เพื่อเพิ่มข้อมูลลูกค้า
                        DataCustomers.AddDataCustomers(IdCustomers, NameCustomers, AddressCustomers, EmailCustomers);
                    }
                    else
                    {
                        // If the customer exists, update their information
                        DataCustomers.UpdateDataCustomers(IdCustomers, NameCustomers, AddressCustomers, EmailCustomers);
                    }

                    // รีเฟรชข้อมูล
                    RefreshData();
                }
                else
                {
                    MessageBox.Show("กรุณากรอกข้อมูลให้ครบทุกช่อง");
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ถูกต้องในช่อง 'ID'");
            }
        }

        // เมธอดเพื่อตรวจสอบว่าหมายเลขลูกค้าหรืออีเมล์ซ้ำกันในฐานข้อมูลหรือไม่
        private bool IsCustomerExists(int id, string email)
        {
            string sql = "SELECT COUNT(*) FROM Customers WHERE Customer_id = @CustomerId OR Email = @Email";

            using (SqliteConnection conn = new SqliteConnection("Data Source=Bookstore.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", id);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private void deleteCustomers_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridCustomers.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)DataGridCustomers.SelectedItem;
                int customerId = int.Parse(selectedRow["Customer_id"].ToString());

                // ลบที่เลือกข้อมูลจาก DataGrid
                DeleteCustomerFromDataGrid(customerId);

                // ล้างข้อมูล ใน TextBoxes
                idCustomers.Text = "";
                nameCustomers.Text = "";
                addressCustomers.Text = "";
                emailCustomers.Text = "";
            }
        }

        private void DeleteCustomerFromDataGrid(int customerId)
        {
            string deleteSql = "DELETE FROM Customers WHERE Customer_id = @CustomerId";

            using (SqliteConnection conn = new SqliteConnection("Data Source=Bookstore.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = new SqliteCommand(deleteSql, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }

            // รีเฟรชข้อมูล
            RefreshData();
        }

        private void DataGridCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            idCustomers.IsReadOnly = true;
            if (DataGridCustomers.SelectedItem != null)
            {

                DataRowView selectedRow = (DataRowView)DataGridCustomers.SelectedItem;

                if (int.TryParse(selectedRow["Customer_id"].ToString(), out int customerId))
                {
                    string customerName = (string)selectedRow["Customer_Name"];
                    string address = (string)selectedRow["Address"];
                    string email = (string)selectedRow["Email"];

                    idCustomers.Text = customerId.ToString();
                    nameCustomers.Text = customerName;
                    addressCustomers.Text = address;
                    emailCustomers.Text = email;
                }
                else
                {
                    MessageBox.Show("ไม่สามารถแปลงรหัสลูกค้าได้");
                }
            }
            else
            {
                // ล้างข้อมูล ใน TextBoxes
                idCustomers.Text = "";
                nameCustomers.Text = "";
                addressCustomers.Text = "";
                emailCustomers.Text = "";
            }
        }
    }
}
