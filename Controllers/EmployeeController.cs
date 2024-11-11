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
            InventoryObjectModel inventory = new InventoryObjectModel {ItemName = "bob", ItemDescription = "bob", ItemPrice = 50.99, Qty = 5, InStock = false};
            InventoryObjectModel inventory2 = new InventoryObjectModel {ItemName = "john", ItemDescription = "john", ItemPrice = 69.99, Qty = 72, InStock = false};
            InventoryObjectModel[] test = {inventory, inventory2};
            ViewBag.items = test;
            return View();
        }

        public IActionResult Supplies() {
            InventoryObjectModel supply1 = new InventoryObjectModel {ItemName = "bob", ItemDescription = "bob", ItemPrice = 50.99, Qty = 5, InStock = true};
            InventoryObjectModel supply2 = new InventoryObjectModel {ItemName = "john", ItemDescription = "john", ItemPrice = 69.99, Qty = 72, InStock = true};
            InventoryObjectModel supply3 = new InventoryObjectModel {ItemName = "jeb", ItemDescription = "jeb", ItemPrice = 120.99, Qty = 1, InStock = true};
            InventoryObjectModel supply4 = new InventoryObjectModel {ItemName = "oops", ItemDescription = "out of stock item", ItemPrice = 1.01, Qty = 0, InStock = false};
            InventoryObjectModel[] test = {supply1, supply2, supply3, supply4};
            ViewBag.availableItems = test;
            return View();
        }

        public IActionResult TransactionHistory() {
            TransactionObjectModel transaction1 = new TransactionObjectModel {Date = DateTime.Now, Amount = 40.99, ID = 1, IsSale = false};
            TransactionObjectModel transaction2 = new TransactionObjectModel {Date = DateTime.Now, Amount = 50.99, ID = 2, IsSale = false};
            TransactionObjectModel transaction3 = new TransactionObjectModel {Date = new DateTime(12345678910), Amount = 78.99, ID = 3, IsSale = true};
            TransactionObjectModel[] test = {transaction1, transaction2, transaction3};
            ViewBag.transactions = test;
            return View();
        }
    }
}