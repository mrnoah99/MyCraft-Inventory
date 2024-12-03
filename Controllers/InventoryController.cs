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

            var model = new OrderItemsCompositeModel {
                MainPageModel =  dbData,
                CartModel = new CartItemViewModel { Description="", Name="", UserId="", Price=0, Quantity=0 }
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> OrderItems(string Name, int Quantity, string Description, double Price) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            string UserId = user.Id;
            if (UserId != null && Name != null && Description != null) {
                var exists = await _context.CartItems.FirstOrDefaultAsync(i => i.UserId == user.Id && i.Name == Name);
                var product = await _context.Products.FirstOrDefaultAsync(i => i.Name == Name);
                if (product != null && product.Quantity < Quantity) {
                    Quantity = product.Quantity;
                    product.Quantity = 0;
                    _context.Update(product);
                }
                if (exists != null) {
                    exists.Quantity += Quantity;
                    if (product != null && product.Quantity != 0) {
                        product.Quantity -= Quantity;
                        _context.Products.Update(product);
                    }
                    _context.CartItems.Update(exists);
                } else {
                    UserId = user.Id;
                    if (product != null) {
                        product.Quantity -= Quantity;
                        _context.Products.Update(product);
                    }
                    CartItemViewModel model = new CartItemViewModel { Name=Name, Quantity=Quantity, UserId=UserId, Description=Description, Price=Price };
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
            var dbData = await _context.CartItems.ToListAsync();
            List<CartItemViewModel> cartList = [];
            dbData.ForEach((item) => {
                if (item.UserId == user.Id) cartList.Add(item);
                Console.WriteLine(cartList[0].UserId);
            });
            var model = new CartHandlerCompositeModel { CartList=cartList, CartRemove=new CartItemViewModel { Description="", Name="", UserId=user.Id }, Payment=new PaymentModel { CardNumber=0, CVV=0, ExpirationDate="", FirstName="", Items=[], LastName="" }};
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int Quantity, string Name, string UserId) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var dbData = await _context.CartItems.Where(e => e.UserId == user.Id).ToListAsync();
            var cartItemToRemove = await _context.CartItems.FirstOrDefaultAsync(i => i.Name == Name && i.UserId == UserId);
            var product = await _context.Products.FirstOrDefaultAsync(i => i.Name == Name);
            if (cartItemToRemove != null && product != null) {
                cartItemToRemove.Quantity -= Quantity;
                product.Quantity += Quantity;
                _context.CartItems.Update(cartItemToRemove);
                _context.Products.Update(product);
                if (cartItemToRemove.Quantity == 0) {
                    _context.CartItems.Remove(cartItemToRemove);
                }
                await _context.SaveChangesAsync();
                TempData["Message"] = "Remove from cart successful.";
                return RedirectToAction("Cart", "Inventory");
            } else {
                Console.WriteLine("MODEL NAME: " + Name + "\nMODEL ID: " + UserId);
                TempData["Message"] = "An error occurred, please try again later.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment() {
            var user = await _userManager.GetUserAsync(User);
            if (user != null) {
                var dbData = await _context.CartItems.ToListAsync();
                List<CartItemViewModel> Items = [];
                dbData.ForEach((item) => {
                    if (item.UserId == user.Id) Items.Add(item);
                });
                Items.ForEach(async (item) => {
                    var newTransaction = new TransactionObjectModel { Amount=item.Quantity*item.Price, Date=DateTime.Now, ID=default, IsSale=true, UserId=user.Id, ProductName=item.Name };
                    _context.TransactionHistory.Add(newTransaction);
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                });
            }
            TempData["Message"] = "Cart checkout complete. Thank you, come again!";
            return RedirectToAction("Index", "Home");
        }
    }
}
