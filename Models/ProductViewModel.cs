namespace MyCraft_Inventory.Models
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public bool IsInStock => Quantity > 0;
    }
}
