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
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderItems(CartItemViewModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            if (ModelState.IsValid) {
                var exists = await _context.CartItems.FirstOrDefaultAsync(i => i.UserId == model.UserId && i.Name == model.Name);
                var product = await _context.Products.FirstOrDefaultAsync(i => i.Name == model.Name);
                if (product != null && product.Quantity < model.Quantity) {
                    model.Quantity = product.Quantity;
                    product.Quantity = 0;
                    _context.Update(product);
                }
                if (exists != null) {
                    exists.Quantity += model.Quantity;
                    if (product != null && product.Quantity != 0) {
                        product.Quantity -= model.Quantity;
                        _context.Products.Update(product);
                    }
                    _context.CartItems.Update(exists);
                } else {
                    model.UserId = user.UserName!;
                    if (product != null) {
                        product.Quantity -= model.Quantity;
                        _context.Products.Update(product);
                    }
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

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(CartItemViewModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }
            var dbData = await _context.CartItems.Where(e => e.UserId == user.UserName).ToListAsync();
            var cartItemToRemove = await _context.CartItems.FirstOrDefaultAsync(i => i.Name == model.Name && i.UserId == model.UserId);
            var product = await _context.Products.FirstOrDefaultAsync(i => i.Name == model.Name);
            if (cartItemToRemove != null && product != null) {
                cartItemToRemove.Quantity -= model.Quantity;
                product.Quantity += model.Quantity;
                _context.CartItems.Update(cartItemToRemove);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Remove from cart successful.";
                return RedirectToAction("Cart", "Inventory");
            } else {
                TempData["Message"] = "An error occurred, please try again later.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentModel model) {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && ModelState.IsValid && user.UserName != null) {
                model.Items.ForEach((item) => {
                    var newTransaction = new TransactionObjectModel { Amount=item.Quantity*item.Price, Date=DateTime.Now, ID=default, IsSale=true, UserId=user.UserName};
                    _context.TransactionHistory.Add(newTransaction);
                    _context.CartItems.Remove(item);
                });
            }
            TempData["Message"] = "Cart checkout complete. Thank you, come again!";
            return RedirectToAction("Index", "Home");
        }
    }
}
