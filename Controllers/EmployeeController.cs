using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCraft_Inventory.Models;

namespace MyCraft_Inventory.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        // [Authorize]
        public IActionResult Inventory()
        {
            InventoryObjectModel inventory = new InventoryObjectModel {ItemName = "bob", ItemDescription = "bob", ItemPrice = 50.99, Qty = 5, InStock=false};
            InventoryObjectModel inventory2 = new InventoryObjectModel {ItemName = "john", ItemDescription = "john", ItemPrice = 69.99, Qty = 72, InStock=false};
            InventoryObjectModel[] test = {inventory, inventory2};
            ViewBag.items = test;
            return View();
        }
    }
}