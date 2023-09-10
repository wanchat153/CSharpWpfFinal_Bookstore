using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpWpfFinal_Bookstore.Encapsulation
{
    internal class OrderItem
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public double TotalPrice { get; set; }
        public double PricePerPiece { get; set; } // เพิ่ม PricePerPiece

        public double CalculateTotalPrice()
        {
            TotalPrice = Quantity * Price;
            return TotalPrice;
        }
    }
}
