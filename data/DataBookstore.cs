using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpWpfFinal_Bookstore.data
{
    internal class DataBookstore
    {
        public async static void InitializeDataBookstore()
        {
            using (SqliteConnection dbBooks = new SqliteConnection($"Filename=Bookstore.db"))
            {
                dbBooks.Open();
                String tableBooks = "CREATE TABLE IF NOT EXISTS Books(" +
                    "ISBN INTEGER PRIMARY KEY," +
                    "Title TEXT NOT NULL," +
                    "Description TEXT NOT NULL," +
                    "Price REAL NOT NULL)";

                SqliteCommand createTableBooks = new SqliteCommand(tableBooks, dbBooks);
                createTableBooks.ExecuteReader();
                dbBooks.Close();
            }
        }

        public static void AddDataBook(int ISBNBook, string TitleBook, string DescriptionBook, string PriceBook)
        {
            using (SqliteConnection dbBooks = new SqliteConnection($"Filename=Bookstore.db"))
            {
                dbBooks.Open();
                SqliteCommand insertBooks = new SqliteCommand();
                insertBooks.Connection = dbBooks;

                insertBooks.CommandText = "INSERT INTO Books VALUES (@ISBN, @Title, @Description, @Price);";
                insertBooks.Parameters.AddWithValue("@ISBN", ISBNBook);
                insertBooks.Parameters.AddWithValue("@Title", TitleBook);
                insertBooks.Parameters.AddWithValue("@Description", DescriptionBook);
                insertBooks.Parameters.AddWithValue("@Price", PriceBook);
                insertBooks.ExecuteReader();
                dbBooks.Close();
            }
        }

        public static void UpdateDataBook(int ISBNBook, string TitleBook, string DescriptionBook, string PriceBook)
        {
            using (SqliteConnection dbBooks = new SqliteConnection($"Filename=Bookstore.db"))
            {
                dbBooks.Open();
                SqliteCommand updateBooks = new SqliteCommand();
                updateBooks.Connection = dbBooks;

                updateBooks.CommandText = "UPDATE Books SET Title = @Title, Description = @Description, Price = @Price WHERE ISBN = @ISBN;";
                updateBooks.Parameters.AddWithValue("@ISBN", ISBNBook);
                updateBooks.Parameters.AddWithValue("@Title", TitleBook);
                updateBooks.Parameters.AddWithValue("@Description", DescriptionBook);
                updateBooks.Parameters.AddWithValue("@Price", PriceBook);
                updateBooks.ExecuteNonQuery();
                dbBooks.Close();
            }
        }

        public static List<string> GetISBNs()
        {
            List<string> isbns = new List<string>();

            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                SqliteCommand getISBNs = new SqliteCommand("SELECT ISBN FROM Books", db);
                using (SqliteDataReader reader = getISBNs.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        isbns.Add(reader["ISBN"].ToString());
                    }
                }
                db.Close();
            }

            return isbns;
        }
    }
}
