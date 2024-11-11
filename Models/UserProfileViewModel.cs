using System.Collections.Generic;

namespace MyCraft_Inventory.Models
{
    public class UserProfileViewModel
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string AccountId { get; set; }
        public required string Role { get; set; }
        public required List<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        public required string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public required string Date { get; set; }
    }
}
