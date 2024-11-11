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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Username, Email = model.Email };
                if (model.Password == model.ConfirmPassword)
                {
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
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
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect if user is not logged in
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var role = isAdmin ? "Admin" : "User";

            // Example transactions; replace with actual data fetching logic
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = "TX12345", Amount = 100.00M, Date = "2024-01-01" },
                new Transaction { TransactionId = "TX67890", Amount = 50.50M, Date = "2024-01-10" }
            };

            var model = new UserProfileViewModel
            {
                Email = user?.Email ?? string.Empty,
                Username = user?.UserName ?? string.Empty,
                AccountId = user?.Id ?? string.Empty,
                Role = role,
                Transactions = transactions
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> EmployeeProfile()
        {
            var employee = await _userManager.GetUserAsync(User);
            if (employee == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var isAdmin = await _userManager.IsInRoleAsync(employee, "Admin");
            var role = isAdmin ? "Admin" : "Employee";

            // Example transactions; replace with actual data fetching logic
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionId = "RX43563", Amount = 260.00M, Date = "2024-01-01" },
                new Transaction { TransactionId = "RX65816", Amount = 460.50M, Date = "2024-01-10" }
            };

            var model = new EmployeeProfileViewModel
            {
                Email = employee?.Email ?? string.Empty,
                Username = employee?.UserName ?? string.Empty,
                AccountId = employee?.Id ?? string.Empty,
                Role = role,
                Transactions = transactions
            };

            return View(model);
        }

    }
}
