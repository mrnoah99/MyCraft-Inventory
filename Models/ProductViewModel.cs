using System.ComponentModel.DataAnnotations;

namespace MyCraft_Inventory.Models
{
    public class ProductViewModel
    {
        [Key]
        public required int ID { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsInStock => Quantity > 0;
        
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }
    }
}
