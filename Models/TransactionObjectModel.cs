using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class TransactionObjectModel
    {
        public required DateTime Date {get; set;}

        public required int ID {get; set;}

        public required double Amount {get; set;}

        public required bool IsSale {get; set;}
    }
}