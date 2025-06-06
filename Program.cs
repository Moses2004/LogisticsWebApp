using LogisticsWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;


var builder = WebApplication.CreateBuilder(args);

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure EF + Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// FIX: Ensure AddRoles<IdentityRole>() is explicitly chained for role support.
// The order matters here: .AddRoles should be before .AddEntityFrameworkStores in some scenarios,
// or at least explicitly present to register the correct Identity services.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // <--- ENSURE THIS IS EXPLICITLY HERE!
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
var app = builder.Build();

// === START: Role and User Seeding for Development ===
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roleNames = { "Admin", "Manager", "Client" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"Role '{roleName}' created.");
        }
        else
        {
            Console.WriteLine($"Role '{roleName}' already exists. Skipping creation.");
        }
    }

    var managerUserEmail = "moses@gmail.com"; // Your actual manager email
    var managerUser = await userManager.FindByEmailAsync(managerUserEmail);
    if (managerUser != null)
    {
        if (!await userManager.IsInRoleAsync(managerUser, "Manager"))
        {
            await userManager.AddToRoleAsync(managerUser, "Manager");
            Console.WriteLine($"User '{managerUserEmail}' assigned to 'Manager' role.");
        }
        else
        {
            Console.WriteLine($"User '{managerUserEmail}' is already in 'Manager' role.");
        }
    }
    else
    {
        Console.WriteLine($"User '{managerUserEmail}' not found. Cannot assign 'Manager' role.");
    }

    var allUsers = await userManager.Users.ToListAsync();
    foreach (var user in allUsers)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        if (!userRoles.Any())
        {
            await userManager.AddToRoleAsync(user, "Client");
            Console.WriteLine($"User '{user.Email}' assigned to 'Client' role (no existing roles found).");
        }
        else if (userRoles.Contains("Client"))
        {
            Console.WriteLine($"User '{user.Email}' is already a 'Client'.");
        }
        else
        {
            Console.WriteLine($"User '{user.Email}' already has roles: {string.Join(", ", userRoles)}. Skipping 'Client' assignment.");
        }
    }
}
// === END: Role and User Seeding ===


// Middleware pipeline
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

// app.MapIdentityApi<IdentityUser>(); // Keep this commented out for now to ensure logout works if it was the conflict

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


app.Run();
