namespace MyCraft_Inventory.Models
{
    public class ProductViewModel
    {
        public required string Name { get; set; }
        public int Quantity { get; set; }
        public required string Description { get; set; }
        public bool IsInStock => Quantity > 0;
    }
}