using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using MyCraft_Inventory.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace MyCraft_Inventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect if user is not logged in
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Employee");
            var role = isAdmin ? "Admin" : "User";

            if (isAdmin) {
                // Example transactions; replace with actual data fetching logic
                var transactions = new List<TransactionObjectModel>
                {
                    new TransactionObjectModel { ID = 43563, Amount = 260.00, Date = new DateTime(new DateOnly(2024, 1, 1), new TimeOnly(12, 30)), IsSale = false },
                    new TransactionObjectModel { ID = 65816, Amount = 460.50, Date = new DateTime(new DateOnly(2024, 1, 10), new TimeOnly(11, 25)), IsSale = true }
                };

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
                var transactions = new List<TransactionObjectModel>
                {
                    new TransactionObjectModel { ID = 12345, Amount = 100.00, Date = new DateTime(new DateOnly(2024, 1, 1), new TimeOnly(14, 50)), IsSale = true },
                    new TransactionObjectModel { ID = 67890, Amount = 50.50, Date = new DateTime(new DateOnly(2024, 1, 10), new TimeOnly(12, 30)), IsSale = true }
                };

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
