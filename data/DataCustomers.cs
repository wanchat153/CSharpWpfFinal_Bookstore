using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpWpfFinal_Bookstore.data
{
    internal class DataCustomers
    {
        public async static void InitializeDataCustomers()
        {
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                String tableCustomers = "CREATE TABLE IF NOT EXISTS Customers(" +
                    "Customer_id INTEGER PRIMARY KEY," +
                    "Customer_Name TEXT NOT NULL," +
                    "Address TEXT NOT NULL," +
                    "Email TEXT NOT NULL)";

                SqliteCommand createTableCustomers = new SqliteCommand(tableCustomers, db);
                createTableCustomers.ExecuteReader();
                db.Close();
            }
        }

        public static void AddDataCustomers(int IdCustomers, string NameCustomers, string AddressCustomers, string EmailCustomers)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                SqliteCommand insertCustomers = new SqliteCommand();
                insertCustomers.Connection = db;

                insertCustomers.CommandText = "INSERT INTO Customers VALUES (@Customer_id, @Customer_Name, @Address, @Email);";
                insertCustomers.Parameters.AddWithValue("@Customer_id", IdCustomers);
                insertCustomers.Parameters.AddWithValue("@Customer_Name", NameCustomers);
                insertCustomers.Parameters.AddWithValue("@Address", AddressCustomers);
                insertCustomers.Parameters.AddWithValue("@Email", EmailCustomers);
                insertCustomers.ExecuteReader();
                db.Close();
            }
        }

        public static void UpdateDataCustomers(int IdCustomers, string NameCustomers, string AddressCustomers, string EmailCustomers)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                SqliteCommand updateCustomers = new SqliteCommand();
                updateCustomers.Connection = db;

                updateCustomers.CommandText = "UPDATE Customers SET Customer_Name = @Customer_Name, Address = @Address, Email = @Email WHERE Customer_id = @Customer_id;";
                updateCustomers.Parameters.AddWithValue("@Customer_id", IdCustomers);
                updateCustomers.Parameters.AddWithValue("@Customer_Name", NameCustomers);
                updateCustomers.Parameters.AddWithValue("@Address", AddressCustomers);
                updateCustomers.Parameters.AddWithValue("@Email", EmailCustomers);
                updateCustomers.ExecuteNonQuery();
                db.Close();
            }
        }

        public static List<string> GetCustomerIDs()
        {
            List<string> customerIDs = new List<string>();

            using (SqliteConnection db = new SqliteConnection($"Filename=Bookstore.db"))
            {
                db.Open();
                SqliteCommand getCustomerIDs = new SqliteCommand("SELECT Customer_id FROM Customers", db);
                using (SqliteDataReader reader = getCustomerIDs.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerIDs.Add(reader["Customer_id"].ToString());
                    }
                }
                db.Close();
            }

            return customerIDs;
        }
    }
}
