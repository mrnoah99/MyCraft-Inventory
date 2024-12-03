namespace MyCraft_Inventory.Models
{
    public class EmployeeProfileViewModel
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string AccountId { get; set; }
        public required string Role { get; set; }
        public required List<Transaction> Transactions { get; set; }
    }
}
