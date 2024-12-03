using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCraft_Inventory.Models;
using Microsoft.AspNetCore.Identity;

namespace MyCraft_Inventory.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Inventory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                InventoryObjectModel inventory = new InventoryObjectModel {ItemName = "bob", ItemDescription = "bob", ItemPrice = 50.99, Qty = 5, InStock = false};
                InventoryObjectModel inventory2 = new InventoryObjectModel {ItemName = "john", ItemDescription = "john", ItemPrice = 69.99, Qty = 72, InStock = false};
                InventoryObjectModel[] test = {inventory, inventory2};
                ViewBag.items = test;
                return View();
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Supplies() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                InventoryObjectModel supply1 = new InventoryObjectModel {ItemName = "bob", ItemDescription = "bob", ItemPrice = 50.99, Qty = 5, InStock = true};
                InventoryObjectModel supply2 = new InventoryObjectModel {ItemName = "john", ItemDescription = "john", ItemPrice = 69.99, Qty = 72, InStock = true};
                InventoryObjectModel supply3 = new InventoryObjectModel {ItemName = "jeb", ItemDescription = "jeb", ItemPrice = 120.99, Qty = 1, InStock = true};
                InventoryObjectModel supply4 = new InventoryObjectModel {ItemName = "oops", ItemDescription = "out of stock item", ItemPrice = 1.01, Qty = 0, InStock = false};
                InventoryObjectModel[] test = {supply1, supply2, supply3, supply4};
                ViewBag.availableItems = test;
                return View();
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> TransactionHistory() {var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                TransactionObjectModel transaction1 = new TransactionObjectModel {Date = DateTime.Now, Amount = 40.99, ID = 1, IsSale = false};
                TransactionObjectModel transaction2 = new TransactionObjectModel {Date = DateTime.Now, Amount = 50.99, ID = 2, IsSale = false};
                TransactionObjectModel transaction3 = new TransactionObjectModel {Date = new DateTime(12345678910), Amount = 78.99, ID = 3, IsSale = true};
                TransactionObjectModel[] test = {transaction1, transaction2, transaction3};
                ViewBag.transactions = test;
                return View();
            } else {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}