using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class CartHandlerCompositeModel
    {
        public required List<CartItemViewModel> CartList;
        public required CartItemViewModel CartRemove;
        public required PaymentModel Payment;
    }
}