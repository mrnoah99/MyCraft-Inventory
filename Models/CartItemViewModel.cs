using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCraft_Inventory.Models
{
    public class CartItemViewModel
    {
        [Key]
        public required string Name { get; set; }
        public int Quantity { get; set; }
        public required string Description { get; set; }
        [Key]
        public required string UserId { get; set; }
        public double Price { get; set; }
    }
}