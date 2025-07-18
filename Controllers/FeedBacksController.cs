using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LogisticsWebApp.Data;
using LogisticsWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Add this namespace for Claims

namespace LogisticsWebApp.Controllers
{
    public class FeedBacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedBacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FeedBacks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FeedBacks.Include(f => f.Invoice);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FeedBacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedBack = await _context.FeedBacks
                .Include(f => f.Invoice)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedBack == null)
            {
                return NotFound();
            }

            return View(feedBack);
        }

        // GET: FeedBacks/Create
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            // Filter invoices to show only those belonging to the current user
            var userInvoices = await _context.Invoices
                                             .Where(i => i.CustomerId == userId)
                                             .ToListAsync();

            ViewData["InvoiceID"] = new SelectList(userInvoices, "InvoiceID", "InvoiceID");
            return View();
        }

        // POST: FeedBacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,Manager,Client")] // Only authorized users (e.g., Clients) should create feedback
        public async Task<IActionResult> Create([Bind("FeedbackID,InvoiceID,Rating,Message")] FeedBack feedBack)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            // Validate that the submitted InvoiceID belongs to the current user
            var invoiceExistsForUser = await _context.Invoices.AnyAsync(i => i.InvoiceID == feedBack.InvoiceID && i.CustomerId == userId);

            if (!invoiceExistsForUser)
            {
                ModelState.AddModelError("InvoiceID", "You can only create feedback for your own invoices.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(feedBack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid or invoice doesn't belong to user, re-populate the dropdown
            var userInvoices = await _context.Invoices
                                             .Where(i => i.CustomerId == userId)
                                             .ToListAsync();
            ViewData["InvoiceID"] = new SelectList(userInvoices, "InvoiceID", "InvoiceID", feedBack.InvoiceID);
            return View(feedBack);
        }

        // GET: FeedBacks/Edit/5
        [Authorize(Roles = "Admin,Manager")] // Only Admin and Manager can access this
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedBack = await _context.FeedBacks.FindAsync(id);
            if (feedBack == null)
            {
                return NotFound();
            }
            ViewData["InvoiceID"] = new SelectList(_context.Invoices, "InvoiceID", "InvoiceID", feedBack.InvoiceID);
            return View(feedBack);
        }

        // POST: FeedBacks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")] // Only Admin and Manager can access this
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackID,InvoiceID,Rating,Message")] FeedBack feedBack)
        {
            if (id != feedBack.FeedbackID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedBack);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedBackExists(feedBack.FeedbackID))
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
            ViewData["InvoiceID"] = new SelectList(_context.Invoices, "InvoiceID", "InvoiceID", feedBack.InvoiceID);
            return View(feedBack);
        }

        // GET: FeedBacks/Delete/5
        [Authorize(Roles = "Admin,Manager")] // Only Admin and Manager can access this
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedBack = await _context.FeedBacks
                .Include(f => f.Invoice)
                .FirstOrDefaultAsync(m => m.FeedbackID == id);
            if (feedBack == null)
            {
                return NotFound();
            }

            return View(feedBack);
        }

        // POST: FeedBacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")] // Only Admin and Manager can access this
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedBack = await _context.FeedBacks.FindAsync(id);
            if (feedBack != null)
            {
                _context.FeedBacks.Remove(feedBack);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedBackExists(int id)
        {
            return _context.FeedBacks.Any(e => e.FeedbackID == id);
        }
    }
}