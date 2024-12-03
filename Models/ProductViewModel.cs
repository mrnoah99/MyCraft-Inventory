using System.ComponentModel.DataAnnotations;

namespace MyCraft_Inventory.Models
{
    public class ProductViewModel
    {
        [Key]
        public required int ID { get; set; }
        public required string Name { get; set; }
        public int Quantity { get; set; }
        public required string Description { get; set; }
        public bool IsInStock => Quantity > 0;
        public double Price { get; set; }
    }
}
