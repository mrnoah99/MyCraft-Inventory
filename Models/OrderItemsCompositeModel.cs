using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class OrderItemsCompositeModel
    {
        public List<ProductViewModel> MainPageModel { get; set; }
        public CartItemViewModel CartModel { get; set; }
    }
}