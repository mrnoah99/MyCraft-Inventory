using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyCraft_Inventory.Models;
using Microsoft.AspNetCore.Identity;
using SQLitePCL;
using MyCraft_Inventory.Data;
using Microsoft.EntityFrameworkCore;

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
                List<ProductViewModel> items = await _context.Products.ToListAsync();
                return View(items);
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> Supplies() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                List<ProductViewModel> items = await _context.Products.ToListAsync();
                SupplyCompositeModel model = new() { AllProducts=items, SuppliedProduct=new ProductViewModel { Name="", Description="", ID=default} };
                return View(model);
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseStock(int Quantity, string Name, int Price) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var product = await _context.Products.FirstOrDefaultAsync(i => i.Name == Name);
            if (product != null) {
                product.Quantity += Quantity;
                _context.Products.Update(product);
                var newTransaction = new TransactionObjectModel { Amount=Quantity*Price, Date=DateTime.Now, ID=default, IsSale=false, UserId=user.Id, ProductName=Name };
                await _context.SaveChangesAsync();
                TempData["Message"] = "Stock successfully ordered.";
                return RedirectToAction("Inventory", "Employee");
            } else {
                TempData["Message"] = "An error occurred, please try again later.";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> TransactionHistory() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            if (isAdmin) {
                List<TransactionObjectModel> transactions = await _context.TransactionHistory.ToListAsync();
                List<ApplicationUser> users = [];
                transactions.ForEach(async (t) => {
                    var user = await _userManager.FindByIdAsync(t.UserId);
                    if (user != null) {
                        users.Add(user);
                    }
                });
                ViewBag.users = users;
                return View(transactions);
            } else {
                TempData["Message"] = "You must be an Employee to access this page.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewItem(ProductViewModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid) {
                var newProduct = new ProductViewModel { Name=model.Name, Description=model.Description, Price=model.Price, Quantity=model.Quantity, ID=default };
                _context.Products.Add(newProduct);
                var newTransaction = new TransactionObjectModel { Amount=model.Quantity*model.Price, Date=DateTime.Now, ID=default, IsSale=false, ProductName=model.Name, UserId=user.Id };
                _context.TransactionHistory.Add(newTransaction);
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