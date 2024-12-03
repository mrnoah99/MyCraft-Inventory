using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCraft_Inventory.Data;
using MyCraft_Inventory.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Seed roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DataSeeder.SeedRoles(services);
    }
    catch (Exception ex)
    {
        // Log errors if needed
        Console.WriteLine($"Error seeding roles: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Login",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "Register",
    pattern: "{controller=Account}/{action=Register}/{id?}");

app.MapControllerRoute(
    name: "OrderItems",
    pattern: "{controller=Inventory}/{action=OrderItems}/{id?}");

app.MapControllerRoute(
    name: "Profile",
    pattern: "{controller=Account}/{action=Profile}/{id?}");

app.MapControllerRoute(
    name: "New Item",
    pattern: "{controller=Employee}/{action=NewItem}/{id?}");

app.MapControllerRoute(
    name: "Cart",
    pattern: "{controller=Inventory}/{action=Cart}/{id?}");

app.Run();
