// Controllers/OrderlinesController.cs
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticsWebApp.Data;
using LogisticsWebApp.Models;

[Authorize]
public class OrderlinesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public OrderlinesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Orderlines/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Orderlines/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("InitialAddress,DestinationAddress,Date,WeightKg,Status")] Orderline orderline)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError(string.Empty, "You must be logged in to create an order.");
            Console.WriteLine("User ID not found when trying to create order.");
            return View(orderline);
        }

        orderline.CustomerID = userId;

        // CRUCIAL FIX: Clear model state errors for CustomerID and Customer
        // as they are set programmatically, not from the form.
        ModelState.Remove("CustomerID");
        ModelState.Remove("Customer"); // Clear errors for the navigation property too

        // Now, re-evaluate ModelState.IsValid. It should pass if other fields are valid.
        if (ModelState.IsValid)
        {
            _context.Add(orderline);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details for server-side debugging
                Console.WriteLine($"Database save failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                ModelState.AddModelError(string.Empty, "An error occurred while saving your order. Please try again.");
                return View(orderline); // Return view with error message
            }

            return RedirectToAction(nameof(MyOrders));
        }

        // If we reach here, ModelState is invalid due to OTHER fields (not CustomerID/Customer)
        // Return the view with the model to display validation messages for other fields.
        return View(orderline);
    }

    // Example MyOrders action
    public async Task<IActionResult> MyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(); // Or redirect to login
        }

        var userOrders = await _context.Orderlines
                                       .Where(o => o.CustomerID == userId)
                                       .OrderByDescending(o => o.Date)
                                       .ToListAsync();

        return View(userOrders);
    }
}
