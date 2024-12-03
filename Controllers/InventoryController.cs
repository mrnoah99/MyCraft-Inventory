using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCraft_Inventory.Data;
using MyCraft_Inventory.Models;
using System.Collections.Generic;

namespace MyCraft_Inventory.Controllers
{
    public class InventoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> OrderItems()
        {
            var dbData = await _context.Products.ToListAsync();
            List<ProductViewModel> products = []; 
            dbData.ForEach(products.Add);
            ViewBag.products = products;

            var model = new OrderItemsCompositeModel {
                MainPageModel =  products,
                CartModel = new CartItemViewModel { Description="", Name="", UserId="", Price=0, Quantity=0 }
            };
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> OrderItems(CartItemViewModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid) {
                var exists = await _context.CartItems.FirstOrDefaultAsync(i => i.UserId == model.UserId);
                if (exists != null) {
                    exists.Quantity += model.Quantity;
                    var product = await _context.Products.FirstOrDefaultAsync(i => i.Name == model.Name);
                    if (product != null && product.Quantity < exists.Quantity) {
                        exists.Quantity = product.Quantity;
                    }
                    _context.Update(exists);
                } else {
                    model.UserId = user.UserName!;
                    _context.CartItems.Add(model);
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = "Added to cart successfully";
                return RedirectToAction("Cart", "Inventory");
            } else {
                TempData["Message"] = "Failed to add to Cart. Please try again later.";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> Cart() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var dbData = await _context.CartItems.Where(e => e.UserId == user.UserName).ToListAsync();
            ViewBag.items = dbData;
            return View();
        }
    }
}
