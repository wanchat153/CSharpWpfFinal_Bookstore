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
    /// Interaction logic for BooksPage.xaml
    /// </summary>
    public partial class BooksPage : Page
    {
        public BooksPage()
        {
            InitializeComponent();

            //เรียกใช้ Sql
            DataBookstore.InitializeDataBookstore();

            RefreshDataBooks();
        }

        //function รีเฟรชข้อมูล
        public void RefreshDataBooks()
        {
            string sqlbooks = "SELECT * FROM Books";

            using (SqliteConnection dbBooks = new SqliteConnection("Data Source=Bookstore.db"))
            {
                dbBooks.Open();

                using (SqliteCommand cmdBooks = new SqliteCommand(sqlbooks, dbBooks))
                {
                    using (SqliteDataReader reader = cmdBooks.ExecuteReader())
                    {
                        DataTable dtBooks = new DataTable();
                        dtBooks.Load(reader);
                        //แสดงข้อมูล SQL ใน DataGrid
                        DataGridBooks.ItemsSource = dtBooks.DefaultView;
                    }
                }
                dbBooks.Close();
            }
        }
        private void searchBooks_TextChanged(object sender, TextChangedEventArgs e)
        {
            //กำหนดตัวแปรใหม่ และ ลบช่องว่าง
            string searchTextBooks = searchBooks.Text.Trim();

            //ตรวสจสอบต้องไม่มีช่องว่าง
            if (!string.IsNullOrWhiteSpace(searchTextBooks))
            {
                //นำข้อมูลที่ได้ มาตรวจสอบ
                SearchBooks(searchTextBooks);
            }
            else
            {
                // รีเฟรชข้อมูล ถ้าไม่มีข้อความค้นหา ให้แสดงข้อมูลทั้งหมด
                RefreshDataBooks();
            }
        }

        //function ค้นหา ID และ Name 
        private void SearchBooks(string searchTextBooks)
        {
            string sqlbooks;
            if (int.TryParse(searchTextBooks, out int ISBNBook))
            {
                // แปลง booksId เป็นข้อความ
                string booksIdText = ISBNBook.ToString();
                sqlbooks = $"SELECT * FROM Books WHERE ISBN = '{booksIdText}' OR Price = '{searchTextBooks}'";
            }
            else
            {
                sqlbooks = "SELECT * FROM Books WHERE Title LIKE @SearchText OR Price = @SearchText";
            }

            using (SqliteConnection dbBooks = new SqliteConnection("Data Source=Bookstore.db"))
            {
                dbBooks.Open();

                using (SqliteCommand cmdBooks = new SqliteCommand(sqlbooks, dbBooks))
                {
                    // _ คือ ไม่ต้องใช้ parameter สำหรับ ISBNBook
                    if (!int.TryParse(searchTextBooks, out int _))
                    {
                        //ค้นหาข้อมูล ตรวจสอบคำที่มีใน ข้อมูล SQL
                        cmdBooks.Parameters.AddWithValue("@SearchText", $"%{searchTextBooks}%");
                    }

                    using (SqliteDataReader reader = cmdBooks.ExecuteReader())
                    {
                        DataTable dtBooks = new DataTable();
                        dtBooks.Load(reader);
                        //แสดงข้อมูล SQL ใน DataGrid
                        DataGridBooks.ItemsSource = dtBooks.DefaultView;
                    }
                }
                dbBooks.Close();
            }
        }
        private void isbnBooks_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void titleBooks_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void descriptionBooks_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void priceBooks_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void addBooks_Click(object sender, RoutedEventArgs e)
        {
            // IsNullOrWhiteSpace ตรวจสอบว่า ทุกช่องต้องไม่มีข้อมูลว่าง
            if (!string.IsNullOrWhiteSpace(isbnBooks.Text) &&
                !string.IsNullOrWhiteSpace(titleBooks.Text) &&
                !string.IsNullOrWhiteSpace(descriptionBooks.Text) &&
                !string.IsNullOrWhiteSpace(priceBooks.Text))
            {
                int isbn = int.Parse(isbnBooks.Text);
                string title = titleBooks.Text;
                string description = descriptionBooks.Text;
                string price = priceBooks.Text;

                //เรียกใช้ funcion IsBookDuplicate ตรวจสอบ isbn และ title
                if (!IsBookDuplicate(isbn, title))
                {
                    // เพิ่มข้อมูลลูกค้าใหม่
                    DataBookstore.AddDataBook(isbn, title, description, price);

                    // รีเฟรชข้อมูล
                    RefreshDataBooks();

                    // ล้างข้อมูล ใน TextBoxes
                    ClearTextBooks();
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

        //funcion ตรวจสอบ ISBN และ Title ไม่ซ้ำกัน
        private bool IsBookDuplicate(int ISBNBook, string TitleBook)
        {
            using (SqliteConnection dbBooks = new SqliteConnection($"Filename=Bookstore.db"))
            {
                dbBooks.Open();
                //ตรวจสอบ ID และ Name ไม่ซ้ำกัน
                string sqlbooks = "SELECT COUNT(*) FROM Books WHERE ISBN = @ISBN OR Title = @Title";
                using (SqliteCommand cmdbooks = new SqliteCommand(sqlbooks, dbBooks))
                {
                    //ค่าของ ISBN และ Title ตรวจสอบในข้อมูล Sql ISBN และ Title
                    cmdbooks.Parameters.AddWithValue("@ISBN", ISBNBook);
                    cmdbooks.Parameters.AddWithValue("@Title", TitleBook);

                    //ถ้าข้อมูลชำกันจะนับ
                    long countBooks = (long)cmdbooks.ExecuteScalar();

                    //ส่งข้อมูลกลับ
                    return countBooks > 0;
                }
            }
        }

        private void deleteBooks_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBooks.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)DataGridBooks.SelectedItem;
                int bookISBN = int.Parse(selectedRow["ISBN"].ToString());

                // ลบที่เลือกข้อมูลจาก DataGrid
                DeleteBooksFromDataGrid(bookISBN);

                // ล้างข้อมูล ใน TextBoxes
                ClearTextBooks();

                // รีเฟรชข้อมูล
                RefreshDataBooks();

                SearchBooks(searchBooks.Text);
            }
        }

        private void DeleteBooksFromDataGrid(int ISBNBook)
        {
            string deleteSqlBook = "DELETE FROM Books WHERE ISBN = @ISBNBook";

            using (SqliteConnection dbBooks = new SqliteConnection("Data Source=Bookstore.db"))
            {
                dbBooks.Open();

                using (SqliteCommand cmdBooks = new SqliteCommand(deleteSqlBook, dbBooks))
                {
                    cmdBooks.Parameters.AddWithValue("@ISBNBook", ISBNBook);
                    cmdBooks.ExecuteNonQuery();
                }

                dbBooks.Close();
            }

            // รีเฟรชข้อมูล
            RefreshDataBooks();

        }

        private void updateBooks_Click(object sender, RoutedEventArgs e)
        {
            // IsNullOrWhiteSpace ตรวจสอบว่า ทุกช่องต้องไม่มีข้อมูลว่าง
            if (!string.IsNullOrWhiteSpace(isbnBooks.Text) &&
                !string.IsNullOrWhiteSpace(titleBooks.Text) &&
                !string.IsNullOrWhiteSpace(descriptionBooks.Text) &&
                !string.IsNullOrWhiteSpace(priceBooks.Text))
            {
                if (int.TryParse(isbnBooks.Text, out int bookISBN))
                {
                    string title = titleBooks.Text;
                    string description = descriptionBooks.Text;
                    string price = priceBooks.Text;

                    DataBookstore.UpdateDataBook(bookISBN, title, description, price);

                    // รีเฟรชข้อมูล
                    RefreshDataBooks();

                    // ล้างข้อมูลใน TextBoxes
                    ClearTextBooks();
                }
                else
                {
                    MessageBox.Show("รหัสหนังสือไม่ถูกต้อง");
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วน");
            }
        }

        private void clearBooks_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBooks();
        }

        private void DataGridBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridBooks.SelectedItem != null)
            {
                // ตรวจสอบชนิดของอ็อบเจ็กต์ที่ถูกเลือก
                if (DataGridBooks.SelectedItem is DataRowView selectedRow)
                {
                    isbnBooks.IsReadOnly = true;
                    titleBooks.IsReadOnly = true;
                    // ตรวจสอบข้อมูลแถวที่เลือก
                    int ISBNbook = int.Parse(selectedRow["ISBN"].ToString());
                    string Titlebook = (string)selectedRow["Title"];
                    string Descriptionbook = (string)selectedRow["Description"];
                    double Pricebook = Convert.ToDouble(selectedRow["Price"]); // แปลงให้เป็น double

                    // ทำตามต้องการกับข้อมูลที่ได้
                    isbnBooks.Text = ISBNbook.ToString();
                    titleBooks.Text = Titlebook;
                    descriptionBooks.Text = Descriptionbook;
                    priceBooks.Text = Pricebook.ToString(); // แปลงให้เป็น string
                }
                else
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการแปลงชนิดข้อมูล");
                }
            }
            else
            {
                // ล้างข้อมูลใน TextBoxes
                ClearTextBooks();
            }
        }

        //function ล้างข้อมูล ใน TextBoxes
        private void ClearTextBooks()
        {
            //กำหนดให้เป็น ค่าว่าง
            isbnBooks.Text = "";
            titleBooks.Text = "";
            descriptionBooks.Text = "";
            priceBooks.Text = "";

            //ทำให้สามารถแก้ไข Textbox ได้
            isbnBooks.IsReadOnly = false;
            titleBooks.IsReadOnly = false;
        }
    }
}
