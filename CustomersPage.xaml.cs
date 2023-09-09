using CSharpWpfFinal_Bookstore.data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
using System.Xml.Linq;

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
            //เรียกใช้ Sql
            DataCustomers.InitializeDataCustomers();
            DataBookstore.InitializeDataBookstore();
            DataOders.InitializeDataOders();

            // รีเฟรชข้อมูล
            RefreshData();
        }

        //function รีเฟรชข้อมูล
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
                        //แสดงข้อมูล SQL ใน DataGrid
                        DataGridCustomers.ItemsSource = dt.DefaultView;
                    }
                }
                conn.Close();
            }
        }

        private void searchCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {
            //กำหนดตัวแปรใหม่ และ ลบช่องว่าง
            string searchText = searchCustomers.Text.Trim();

            //ตรวสจสอบต้องไม่มีช่องว่าง
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                //นำข้อมูลที่ได้ มาตรวจสอบ
                SearchCustomers(searchText);
            }
            else
            {
                // รีเฟรชข้อมูล ถ้าไม่มีข้อความค้นหา ให้แสดงข้อมูลทั้งหมด
                RefreshData();
            }
        }

        //function ค้นหา ID และ Name 
        private void SearchCustomers(string searchText)
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
                    // _ คือ ไม่ต้องใช้ parameter สำหรับ customerId
                    if (!int.TryParse(searchText, out int _))
                    {
                        //ค้นหาข้อมูล ตรวจสอบคำที่มีใน ข้อมูล SQL
                        cmd.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                    }

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        //แสดงข้อมูล SQL ใน DataGrid
                        DataGridCustomers.ItemsSource = dt.DefaultView;
                    }
                }
                conn.Close();
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
            // IsNullOrWhiteSpace ตรวจสอบว่า ทุกช่องต้องไม่มีข้อมูลว่าง
            if (!string.IsNullOrWhiteSpace(idCustomers.Text) &&
                !string.IsNullOrWhiteSpace(nameCustomers.Text) &&
                !string.IsNullOrWhiteSpace(addressCustomers.Text) &&
                !string.IsNullOrWhiteSpace(emailCustomers.Text))
            {
                int id = int.Parse(idCustomers.Text);
                string name = nameCustomers.Text;
                string address = addressCustomers.Text;
                string email = emailCustomers.Text;

                //เรียกใช้ funcion IsCustomerDuplicate ตรวจสอบ id และ name
                if (!IsCustomerDuplicate(id, name))
                {
                    // เพิ่มข้อมูลลูกค้าใหม่
                    DataCustomers.AddDataCustomers(id, name, address, email);

                    // รีเฟรชข้อมูล
                    RefreshData();

                    // ล้างข้อมูล ใน TextBoxes
                    ClearTextBoxes();
                }
                else
                {
                    MessageBox.Show("รหัสลูกค้าหรือชื่อลูกค้าซ้ำกัน กรุณากรอกรหัสลูกค้าและชื่อลูกค้าที่ไม่ซ้ำกัน");
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วน");
            }
        }

        //funcion ตรวจสอบ ID และ Name ไม่ซ้ำกัน
        private bool IsCustomerDuplicate(int IdCustomers, string NameCustomers)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                //ตรวจสอบ ID และ Name ไม่ซ้ำกัน
                string sql = "SELECT COUNT(*) FROM Customers WHERE Customer_id = @Customer_id OR Customer_Name = @Customer_Name";
                using (SqliteCommand cmd = new SqliteCommand(sql, db))
                {
                    //ค่าของ ID และ Name ตรวจสอบในข้อมูล Sql ID และ Name
                    cmd.Parameters.AddWithValue("@Customer_id", IdCustomers);
                    cmd.Parameters.AddWithValue("@Customer_Name", NameCustomers);
                    
                    //ถ้าข้อมูลชำกันจะนับ
                    long count = (long)cmd.ExecuteScalar();

                    //ส่งข้อมูลกลับ
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
                ClearTextBoxes();

                // รีเฟรชข้อมูล
                RefreshData();

                SearchCustomers(searchCustomers.Text);
            }
        }

        private void DeleteCustomerFromDataGrid(int customerId)
        {
            string deleteSql = "DELETE FROM Customers WHERE Customer_id = @CustomerId";

            using (SqliteConnection db = new SqliteConnection("Data Source=Bookstore.db"))
            {
                db.Open();

                using (SqliteCommand cmd = new SqliteCommand(deleteSql, db))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.ExecuteNonQuery();
                }

                db.Close();
            }

            // รีเฟรชข้อมูล
            RefreshData();

        }

        private void DataGridCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridCustomers.SelectedItem != null)
            {
                // ตรวจสอบชนิดของอ็อบเจ็กต์ที่ถูกเลือก
                if (DataGridCustomers.SelectedItem is DataRowView selectedRow)
                {
                    idCustomers.IsReadOnly = true;
                    // ตรวจสอบข้อมูลแถวที่เลือก
                    int customerId = int.Parse(selectedRow["Customer_id"].ToString());
                    string customerName = (string)selectedRow["Customer_Name"];
                    string address = (string)selectedRow["Address"];
                    string email = (string)selectedRow["Email"];

                    // ทำตามต้องการกับข้อมูลที่ได้
                    idCustomers.Text = customerId.ToString();
                    nameCustomers.Text = customerName;
                    addressCustomers.Text = address;
                    emailCustomers.Text = email;
                }
                else
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการแปลงชนิดข้อมูล");
                }
            }
            else
            {
                // ล้างข้อมูลใน TextBoxes
                ClearTextBoxes();
            }
        }

        private void updateCustomers_Click(object sender, RoutedEventArgs e)
        {
            // IsNullOrWhiteSpace ตรวจสอบว่า ทุกช่องต้องไม่มีข้อมูลว่าง
            if (!string.IsNullOrWhiteSpace(idCustomers.Text) &&
                !string.IsNullOrWhiteSpace(nameCustomers.Text) &&
                !string.IsNullOrWhiteSpace(addressCustomers.Text) &&
                !string.IsNullOrWhiteSpace(emailCustomers.Text))
            {
                if (int.TryParse(idCustomers.Text, out int customerId))
                {
                    string name = nameCustomers.Text;
                    string address = addressCustomers.Text;
                    string email = emailCustomers.Text;

                    DataCustomers.UpdateDataCustomers(customerId, name, address, email);

                    // รีเฟรชข้อมูล
                    RefreshData();

                    // ล้างข้อมูลใน TextBoxes
                    ClearTextBoxes();
                }
                else
                {
                    MessageBox.Show("รหัสลูกค้าไม่ถูกต้อง");
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วน");
            }
        }

        //function ล้างข้อมูล ใน TextBoxes
        private void ClearTextBoxes()
        {
            //กำหนดให้เป็น ค่าว่าง
            idCustomers.Text = "";
            nameCustomers.Text = "";
            addressCustomers.Text = "";
            emailCustomers.Text = "";

            //ทำให้สามารถแก้ไข Textbox ได้
            idCustomers.IsReadOnly = false;
        }

        private void clearCustomers_Click(object sender, RoutedEventArgs e)
        {
            // ล้างข้อมูลใน TextBoxes
            ClearTextBoxes();
        }
    }
}
