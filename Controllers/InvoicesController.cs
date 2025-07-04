using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LogisticsWebApp.Data;
using LogisticsWebApp.Models;
using Microsoft.AspNetCore.Authorization; // Add this
using Microsoft.AspNetCore.Identity; // Add this for UserManager
using System.Security.Claims; // For ClaimTypes.NameIdentifier

namespace LogisticsWebApp.Controllers
{
    // Apply authorization at the controller level
    // This means no one can access any Invoice action unless they are authenticated.
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // Inject UserManager

        public InvoicesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager
        }

        // GET: Invoices
        // Managers can see all invoices. Clients can only see their own.
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user's ID
            var currentUserId = _userManager.GetUserId(User); // Gets the string ID of the logged-in user

            IQueryable<Invoice> invoicesQuery = _context.Invoices
                .Include(i => i.Orderline)
                .Include(i => i.TransportUnit)
                .Include(i => i.Customer); // Include Customer if you need to display customer info

            // Check if the user is in the "Manager" role
            if (User.IsInRole("Manager"))
            {
                // Managers see all invoices
                return View(await invoicesQuery.ToListAsync());
            }
            else if (User.IsInRole("Client"))
            {
                // Clients see only their own invoices
                // Ensure CustomerId is not null before comparing
                return View(await invoicesQuery.Where(i => i.CustomerId == currentUserId).ToListAsync());
            }
            else
            {
                // If the user is authenticated but not in Manager or Client role, deny access
                // Or you could redirect to an "Access Denied" page
                return Forbid();
            }
        }

        // GET: Invoices/Details/5
        // Managers can see any invoice details. Clients can only see their own.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Orderline)
                .Include(i => i.TransportUnit)
                .Include(i => i.Customer) // Include Customer for verification
                .FirstOrDefaultAsync(m => m.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Authorization check for Details:
            if (User.IsInRole("Manager"))
            {
                // Manager can view any invoice
                return View(invoice);
            }
            else if (User.IsInRole("Client"))
            {
                // Client can only view their own invoices
                var currentUserId = _userManager.GetUserId(User);
                if (invoice.CustomerId == currentUserId)
                {
                    return View(invoice);
                }
                else
                {
                    return Forbid(); // Client trying to view someone else's invoice
                }
            }
            else
            {
                return Forbid(); // User authenticated but not in an authorized role
            }
        }

        // GET: Invoices/Create - Only Managers can access
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        { 

            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID");
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container");
            // Populate CustomerId dropdown with all users (or filter to clients)
            ViewData["CustomerId"] = new SelectList(_userManager.Users, "Id", "Email"); // Assuming you want to display email
            return View();
        }

        // POST: Invoices/Create - Only Managers can access
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("InvoiceID,OrderlineID,TransportUnitID,TotalAmount,PaymentStatus,CustomerId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                // If CustomerId is not bound from the form, you might need to set it here
                // if (string.IsNullOrEmpty(invoice.CustomerId))
                // {
                //     // Example: if manager is creating invoice for themselves (unlikely for client invoices)
                //     // invoice.CustomerId = _userManager.GetUserId(User);
                // }

                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID", invoice.OrderlineID);
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container", invoice.TransportUnitID);
            ViewData["CustomerId"] = new SelectList(_userManager.Users, "Id", "Email", invoice.CustomerId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5 - Only Managers can access
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID", invoice.OrderlineID); // Changed "CustomerID" back to "OrderlineID"
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container", invoice.TransportUnitID);
            // ViewData["CustomerId"] = new SelectList(_userManager.Users, "Id", "Email", invoice.CustomerId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5 - Only Managers can access
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceID,OrderlineID,TransportUnitID,TotalAmount,PaymentStatus,CustomerId")] Invoice invoice)
        {
            if (id != invoice.InvoiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID", invoice.OrderlineID); // Changed "CustomerID" back to "OrderlineID"
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container", invoice.TransportUnitID);
            // ViewData["CustomerId"] = new SelectList(_userManager.Users, "Id", "Email", invoice.CustomerId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5 - Only Managers can access
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Orderline)
                .Include(i => i.TransportUnit)
                .Include(i => i.Customer) // Include Customer for verification
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            // Managers can delete any. No specific client check needed here as it's manager-only.
            return View(invoice);
        }

        // POST: Invoices/Delete/5 - Only Managers can access
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceID == id);
        }
    }
}