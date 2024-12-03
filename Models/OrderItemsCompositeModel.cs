using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class OrderItemsCompositeModel
    {
        public required List<ProductViewModel> MainPageModel { get; set; }
        public required CartItemViewModel CartModel { get; set; }
    }
}