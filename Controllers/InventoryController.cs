using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCraft_Inventory.Models;
using System.Collections.Generic;

namespace MyCraft_Inventory.Controllers
{
    public class InventoryController : Controller
    {
        [Authorize]
        public IActionResult OrderItems()
        {
            // Sample data; replace with data from your database or service.
            var products = new List<ProductViewModel>
            {
                new ProductViewModel { Name = "Item 1", Quantity = 10, Description = "Anti dandruff shampoo" },
                new ProductViewModel { Name = "Item 2", Quantity = 0, Description = "A pair of titanium scissors" },
                new ProductViewModel { Name = "Item 3", Quantity = 5, Description = "A bag og M&m's" }
            };

            return View(products);
        }
    }
}
