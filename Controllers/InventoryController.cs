using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCraft_Inventory.Data;
using MyCraft_Inventory.Models;
using System.Collections.Generic;

namespace MyCraft_Inventory.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context) {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> OrderItems()
        {
            var id = 1;
            var remove = await _context.Products.FindAsync(id);
            if (remove != null) {
                _context.Products.Remove(remove);
                await _context.SaveChangesAsync();
            }
            var dummyEntity = new ProductViewModel {ID = 1, Description = "Protects from rain.", Name = "Umbrella", Quantity = 5, Price = 9.99};
            _context.Products.Add(dummyEntity);
            await _context.SaveChangesAsync();
            var dbData = await _context.Products.ToListAsync();
            List<ProductViewModel> products = []; 
            dbData.ForEach(products.Add);

            return View(products);
        }
    }
}
