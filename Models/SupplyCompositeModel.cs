using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class SupplyCompositeModel
    {
        public required List<ProductViewModel> AllProducts;
        public required ProductViewModel SuppliedProduct;
    }
}