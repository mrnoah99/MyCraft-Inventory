using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class InventoryObjectModel
    {
        public required string ItemName {get; set;}

        public required int Qty {get; set;}

        public required string ItemDescription {get; set;}

        public required double ItemPrice {get; set;}

        public required bool InStock {get; set;}
    }
}