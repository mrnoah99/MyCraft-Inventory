using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using MyCraft_Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using MyCraft_Inventory.Data;
using Microsoft.EntityFrameworkCore;

namespace MyCraft_Inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task initializeRoles() {
            string[] roles = {"Employee", "Customer"};
            foreach (var role in roles) {
                if (!await _roleManager.RoleExistsAsync(role)) {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user;
                if (model.IsEmployee && !string.IsNullOrEmpty(model.EmployeeCode)) {
                    user = new ApplicationUser { UserName = model.Username, Email = model.Email, EmployeeCode = model.EmployeeCode, IsEmployee = model.IsEmployee };
                } else {
                    user = new ApplicationUser { UserName = model.Username, Email = model.Email, EmployeeCode = "", IsEmployee = false };
                }
                if (model.Password == model.ConfirmPassword)
                {
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        if (model.IsEmployee
                        && !string.IsNullOrEmpty(model.EmployeeCode)) {
                            await _userManager.AddToRoleAsync(user, "Employee");
                        } else {
                            await _userManager.AddToRoleAsync(user, "Customer");
                        }
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model); 
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                TempData["Message"] = "Please log in to access this page.";
                return RedirectToAction("Login", "Account");
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            var role = isAdmin ? "Admin" : "User";

            if (isAdmin) {
                // Example transactions; replace with actual data fetching logic
                List<TransactionObjectModel> transactions = await _context.TransactionHistory.Where(t => t.UserId == user.Id).ToListAsync();

                var model = new ProfileViewModel
                {
                    Email = user?.Email ?? string.Empty,
                    Username = user?.UserName ?? string.Empty,
                    AccountId = user?.Id ?? string.Empty,
                    Role = role,
                    Transactions = transactions
                };
                return View(model);
            } else {
                // Example transactions; replace with actual data fetching logic
                List<TransactionObjectModel> transactions = await _context.TransactionHistory.Where(t => t.UserId == user.Id).ToListAsync();

                var model = new ProfileViewModel
                {
                    Email = user?.Email ?? string.Empty,
                    Username = user?.UserName ?? string.Empty,
                    AccountId = user?.Id ?? string.Empty,
                    Role = role,
                    Transactions = transactions
                };
                return View(model);
            }
        }
    }
}
