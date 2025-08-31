using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Model
{
    public class SalesModel
    {

        public string? BillId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Change { get; set; }
        public string? CashierName { get; set; }
        public DateOnly DateOfTransaction { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
