using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCraft_Inventory.Models;
using Microsoft.AspNetCore.Identity;
using SQLitePCL;
using MyCraft_Inventory.Data;

namespace MyCraft_Inventory.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _context;

        public EmployeeController(ILogger<EmployeeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Inventory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                return View();
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Supplies() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                return View();
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> TransactionHistory() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                TransactionObjectModel transaction1 = new TransactionObjectModel {Date = DateTime.Now, Amount = 40.99, ID = 1, IsSale = false};
                TransactionObjectModel transaction2 = new TransactionObjectModel {Date = DateTime.Now, Amount = 50.99, ID = 2, IsSale = false};
                TransactionObjectModel transaction3 = new TransactionObjectModel {Date = new DateTime(12345678910), Amount = 78.99, ID = 3, IsSale = true};
                TransactionObjectModel[] test = {transaction1, transaction2, transaction3};
                ViewBag.transactions = test;
                return View();
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewItem(ProductViewModel model) {
            if (ModelState.IsValid) {
                var newProduct = new ProductViewModel { Name=model.Name, Description=model.Description, Price=model.Price, Quantity=model.Quantity, ID=default };
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction("OrderItems", "Inventory");
            } else {
                return RedirectToAction("NewItem", "Employee");
            }
        }

        [Authorize]
        public async Task<IActionResult> NewItem() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");

            if (isAdmin) {
                return View();
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}