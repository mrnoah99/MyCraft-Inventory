using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class PaymentModel
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int CardNumber { get; set; }
        public required int CVV { get; set; }
        public required string ExpirationDate { get; set; }
        public required List<CartItemViewModel> Items { get; set; }
    }
}