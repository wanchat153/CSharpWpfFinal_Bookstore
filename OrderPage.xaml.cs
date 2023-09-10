using CSharpWpfFinal_Bookstore.data;
using CSharpWpfFinal_Bookstore.Encapsulation;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static CSharpWpfFinal_Bookstore.OrderPage;

namespace CSharpWpfFinal_Bookstore
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
            LoadISBNData();
            LoadCustomerIDData();
        }

        // โหลดข้อมูล ISBN จากฐานข้อมูลและเติม ComboBox ด้วยข้อมูล
        private void LoadISBNData()
        {
            ISBNComboBox.Items.Clear(); // ลบรายการที่มีอยู่ใน ComboBox ทั้งหมด

            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                string query = "SELECT ISBN FROM Books";

                using (SqliteCommand cmd = new SqliteCommand(query, db))
                {
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // เพิ่ม ISBN ลงใน ComboBox
                            ISBNComboBox.Items.Add(reader["ISBN"].ToString());
                        }
                        // ปิดการอ่านข้อมูลและการเชื่อมต่อกับฐานข้อมูล
                        reader.Close();
                    }
                }
                db.Close();
            }
        }

        // เลือก ISBN จาก ComboBox
        private void ISBNComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ตรวจสอบว่าได้เลือก ISBN หรือยัง
            if (ISBNComboBox.SelectedItem != null)
            {
                string selectedISBN = ISBNComboBox.SelectedItem.ToString();
                // ดึงข้อมูลของลูกค้าโดยใช้ ISBN ที่เลือกจากฐานข้อมูล
                // คุณต้องดำเนินการส่วนนี้เพื่อดึงข้อมูล Title
                Book book = GetBookInformationFromDatabase(selectedISBN);

                // อัปเดต TitleTextBlock ด้วยข้อมูลหนังสือ
                TitleTextBlock.Text = book.Title;
                DescriptionTextBlock.Text = book.Description;
                PriceTextBlock.Text = book.Price.ToString();
            }
        }

        private Book GetBookInformationFromDatabase(string selectedISBN)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                string query = "SELECT Title, Description, Price FROM Books WHERE ISBN = @ISBN";

                using (SqliteCommand cmd = new SqliteCommand(query, db))
                {
                    cmd.Parameters.AddWithValue("@ISBN", selectedISBN);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //สร้าง Book และดึงข้อมูล
                            Book book = new Book
                            {
                                Title = reader["Title"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDouble(reader["Price"])
                            };

                            return book;
                        }
                    }
                }
                db.Close();
            }

            // หากไม่พบหนังสือ ให้ส่งค่าเป็น null
            return null;
        }

        // Load Customer_id data from the database and populate the ComboBox
        private void LoadCustomerIDData()
        {
            CustomerIDComboBox.Items.Clear(); // Clear existing items

            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                string query = "SELECT Customer_id FROM Customers";

                using (SqliteCommand cmd = new SqliteCommand(query, db))
                {
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CustomerIDComboBox.Items.Add(reader["Customer_id"].ToString());
                        }
                        // Close the reader and the database connection
                        reader.Close();
                    }
                }
                db.Close();
            }
        }

        // เลือก Customer_id ใน ComboBox
        private void CustomerIDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ตรวจสอบว่าเลือก Customer_id แล้วหรือยัง
            if (CustomerIDComboBox.SelectedItem != null)
            {
                string selectedCustomerID = CustomerIDComboBox.SelectedItem.ToString();
                // ดึงข้อมูลลูกค้าขึ้นมาโดยใช้ Customer_id ที่เลือกจากฐานข้อมูล
                // คุณต้องดำเนินการในส่วนนี้เพื่อดึงข้อมูล Customer_Name
                string customerName = GetCustomerNameFromDatabase(selectedCustomerID);

                // อัปเดต CustomerNameTextBlock ด้วยข้อชื่อลูกค้า
                CustomerNameTextBlock.Text = customerName;
            }
        }

        private string GetCustomerNameFromDatabase(string selectedCustomerID)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                string query = "SELECT Customer_Name FROM Customers WHERE Customer_id = @CustomerID";

                using (SqliteCommand cmd = new SqliteCommand(query, db))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", selectedCustomerID);

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // ดึงและคืนชื่อลูกค้า
                            string customerName = reader["Customer_Name"].ToString();
                            return customerName;
                        }
                    }
                }
                db.Close();
            }

            // หากไม่พบข้อมูลลูกค้า คืนค่าที่เหมาะสม
            return string.Empty;
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ตรวจสอบว่ามี ISBN ที่ถูกเลือกหรือไม่
            if (ISBNComboBox.SelectedItem != null)
            {
                // รับค่า ISBN ที่ถูกเลือก
                string isbn = ISBNComboBox.SelectedItem.ToString();

                // ดึงราคาหนังสือจากฐานข้อมูลตาม ISBN (คุณต้องอิมพลีเมนต์ฟังก์ชันนี้)
                double bookPrice = GetBookPriceFromDatabase(isbn);

                // แปลงจำนวนที่ผู้ใช้ป้อนเป็นตัวเลข
                if (double.TryParse(QuantityTextBox.Text, out double quantity))
                {
                    // คำนวณราคารวม
                    double totalPrice = bookPrice * quantity;

                    // แสดงราคารวม
                    TotalPriceTextBlock.Text = totalPrice.ToString("C"); // รูปแบบเป็นสกุลเงิน, เช่น, 100.00 บาท
                }
                else
                {
                    // จัดการกรณีป้อนจำนวนไม่ถูกต้อง (เช่น แสดงข้อความข้อผิดพลาด)
                    TotalPriceTextBlock.Text = "จำนวนไม่ถูกต้อง";
                }
            }
        }

        // ดึงราคาของหนังสือจากฐานข้อมูลโดยใช้ ISBN เป็นตัวแปรหลัก
        private double GetBookPriceFromDatabase(string isbn)
        {
            double bookPrice = 0.0;

            // เปิดการเชื่อมต่อกับฐานข้อมูล
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();

                // สร้างคำสั่ง SQL สำหรับดึงราคาหนังสือจากฐานข้อมูล
                string query = "SELECT Price FROM Books WHERE ISBN = @ISBN";

                using (SqliteCommand command = new SqliteCommand(query, db))
                {
                    command.Parameters.AddWithValue("@ISBN", isbn);

                    // ดำเนินการอ่าน sql และรับผลลัพธ์
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // ดึงราคาหนังสือจากผลลัพธ์ของ
                            bookPrice = reader.GetDouble(0);
                        }
                    }
                }

                // ปิดการเชื่อมต่อกับฐานข้อมูล
                db.Close();
            }

            return bookPrice;
        }

        // รายการสั่งซื้อหนังสือที่ผู้ใช้เลือกจะถูกเก็บไว้ใน ObservableCollection นี้
        private ObservableCollection<OrderItem> orderItems = new ObservableCollection<OrderItem>();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // ตรวจสอบว่าทุกข้อมูลถูกกรอกครบและไม่มี ComboBox ใดมีค่าเป็น null
            if (ISBNComboBox.SelectedItem != null &&
                CustomerIDComboBox.SelectedItem != null &&
                !string.IsNullOrEmpty(QuantityTextBox.Text) &&
                int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                if (quantity <= 0)
                {
                    MessageBox.Show("กรุณาป้อนจำนวนเต็มที่มากกว่าศูนย์", "ข้อมูลไม่ถูกต้อง", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // ออกจากเมธอดถ้าจำนวนไม่ถูกต้อง
                }

                string isbn = ISBNComboBox.SelectedItem.ToString();
                string title = TitleTextBlock.Text;
                double price = double.Parse(PriceTextBlock.Text);

                // ดึงค่า CustomerID และ CustomerName จาก ComboBox ของลูกค้า
                string customerID = CustomerIDComboBox.SelectedItem.ToString();
                string customerName = CustomerNameTextBlock.Text;

                // ตรวจสอบว่ามีรายการที่มี ISBN เดียวกันอยู่แล้วหรือไม่
                OrderItem existingItem = orderItems.FirstOrDefault(item => item.ISBN == isbn);

                if (existingItem != null)
                {
                    // หากมีรายการที่มี ISBN เดียวกันอยู่แล้ว ก็บวกจำนวนเพิ่ม
                    existingItem.Quantity += quantity;
                }
                else
                {
                    // หากไม่มีรายการที่มี ISBN เดียวกัน ก็เพิ่มรายการใหม่
                    OrderItem orderItem = new OrderItem
                    {
                        ISBN = isbn,
                        Title = title,
                        Quantity = quantity,
                        Price = price,
                        CustomerID = customerID,
                        CustomerName = customerName
                    };

                    orderItems.Add(orderItem);
                }

                // อัปเดต ListView หรือคอนโทรลอื่นที่แสดงรายการสั่งซื้อ
                UpdateOrderListView();

                // ล้างค่าใน QuantityTextBox หลังจากเพิ่มรายการเสร็จสิ้น
                QuantityTextBox.Text = "";

                // ล้างค่าใน TotalPriceTextBlock
                TotalPriceTextBlock.Text = "";
            }
            else
            {
                // แสดงข้อความแจ้งเตือนถ้าข้อมูลไม่ครบหรือ ComboBox มีค่าเป็น null
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วน", "ข้อมูลไม่ถูกต้อง", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            // ล้างรายการที่มีอยู่ใน ListViewSum
            ListViewSum.ItemsSource = null;

            // คำนวณราคารวมและจำนวนทั้งหมด
            double totalOrderPrice = 0.0;
            int totalQuantity = 0;

            foreach (var item in orderItems)
            {
                // คำนวณราคารวมต่อชิ้นสำหรับแต่ละรายการ
                item.CalculateTotalPrice();

                // เพิ่มราคารวมต่อชิ้นในราคารวมทั้งหมด
                totalOrderPrice += item.TotalPrice;

                // คำนวณราคาต่อชิ้นรวมถึงจำนวน
                item.PricePerPiece = item.Price + (item.Quantity > 0 ? (item.TotalPrice / item.Quantity) : 0);

                // เพิ่มจำนวนรายการในจำนวนทั้งหมด
                totalQuantity += item.Quantity;
            }

            // เพิ่มรายการสรุปด้านล่างที่แสดง "รวมทั้งหมด" ในรายการ
            OrderItem totalSummaryItem = new OrderItem
            {
                Title = "รวมทั้งหมด:", // แสดง "รวมทั้งหมด:"
                TotalPrice = totalOrderPrice, // ให้ TotalPrice แสดงราคาทั้งหมด
                Quantity = totalQuantity // ให้ Quantity แสดงจำนวนรวมของรายการสั่งซื้อทั้งหมด
            };

            // คำนวณราคาต่อชิ้นสำหรับ totalSummaryItem (ราคาต่อชิ้นรวมทั้งหมดของรายการสั่งซื้อทั้งหมด)
            totalSummaryItem.PricePerPiece = totalOrderPrice / totalQuantity;

            // เพิ่มรายการสรุปไปยัง orderItems
            orderItems.Add(totalSummaryItem);

            // แสดงสรุปรายการสั่งซื้อใน ListViewSum
            ListViewSum.ItemsSource = orderItems;
        }

        private void UpdateOrderListView()
        {
            // กรองรายการสรุปรวมออกจาก orderItems
            var orderItemsForListView = orderItems.Where(item => item.Title != "ราคารวม:").ToList();

            OrderListView.ItemsSource = null; // ล้างรายการที่มีอยู่
            OrderListView.ItemsSource = orderItemsForListView; // ตั้งค่ารายการใหม่
        }


        private void ClearOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (orderItems.Count > 0)
            {
                // ลบรายการทั้งหมดใน orderItems
                orderItems.Clear();
                // อัปเดต OrderListView
                UpdateOrderListView();

                // ล้างค่าใน ListViewSum
                ListViewSum.ItemsSource = null;
            }
            else
            {
                // ถ้าไม่มีรายการ แสดงข้อความแจ้งเตือนหรือไม่ให้ทำอะไรก็ได้ตามต้องการ
                MessageBox.Show("ไม่มีรายการในรายการสั่งซื้อ", "ไม่มีรายการ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // ล้างค่าใน ComboBox ของ ISBN
            ISBNComboBox.SelectedIndex = -1;

            // ล้างค่าใน ComboBox ของ ID
            CustomerIDComboBox.SelectedIndex = -1;

            // ล้างค่าใน TextBox ของ Quantity สามารถใช้ string.Empty;
            QuantityTextBox.Text = string.Empty;
            TitleTextBlock.Text = string.Empty;
            DescriptionTextBlock.Text = string.Empty;
            PriceTextBlock.Text = string.Empty;
            CustomerNameTextBlock.Text = string.Empty;
            TotalPriceTextBlock.Text = string.Empty;

            // ล้างค่าใน ListViewSum
            ListViewSum.ItemsSource = null;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewSum.Items.Count > 0)
            {
                // สร้างข้อความสำหรับแสดงข้อมูลลูกค้าเพียงครั้งเดียว
                string customerInfo = $"ID = {orderItems.First().CustomerID}\n";
                customerInfo += $"Name = {orderItems.First().CustomerName}\n";

                // สร้างข้อความสำหรับแสดงรายการสั่งซื้อ
                StringBuilder orderDetails = new StringBuilder();
                double totalOrderPrice = 0.0;

                foreach (OrderItem item in orderItems)
                {
                    // แสดงเฉพาะรายละเอียดของแต่ละรายการรวมถึง TotalPrice เท่านั้น
                    orderDetails.AppendLine($"{item.Title} {item.Quantity}x =  {item.TotalPrice:C}");
                    totalOrderPrice += item.TotalPrice;
                }

                // รวมข้อความทั้งหมดรวมถึง TotalPrice ที่รวมราคาทั้งหมด
                string message = customerInfo + orderDetails.ToString();

                // แสดง MessageBox
                MessageBox.Show(message, "รายการสั่งซื้อ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // ถ้าไม่มีรายการ แสดงข้อความแจ้งเตือนหรือไม่ให้ทำอะไรก็ได้ตามต้องการ
                MessageBox.Show("ไม่มีรายการในรายการสั่งซื้อ", "ไม่มีรายการ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


    }
}
