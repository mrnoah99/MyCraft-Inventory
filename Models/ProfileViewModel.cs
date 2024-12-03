using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class ProfileViewModel
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string AccountId { get; set; }
        public required string Role { get; set; }
        public required List<TransactionObjectModel> Transactions { get; set; }
    }
}