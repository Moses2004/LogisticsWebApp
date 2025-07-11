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

namespace LogisticsWebApp.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ManagerOrderlinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerOrderlinesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManagerOrderlines
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orderlines.Include(o => o.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ManagerOrderlines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderline = await _context.Orderlines
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderlineID == id);
            if (orderline == null)
            {
                return NotFound();
            }

            return View(orderline);
        }

        // GET: ManagerOrderlines/Create
        public IActionResult Create()
        {
            // Correct: Value is "Id" (GUID), Text is "UserName" (email)
            ViewData["CustomerID"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: ManagerOrderlines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderlineID,CustomerID,InitialAddress,DestinationAddress,Date,WeightKg,Status")] Orderline orderline)
        {
            // The previous [Bind] attribute was missing CustomerID. Make sure CustomerID is included.
            // Ensure you do NOT bind the 'Customer' navigation property itself.
            // The CustomerID (string/GUID) is sufficient for linking.

            if (ModelState.IsValid) // This should now pass if no other errors exist
            {
                _context.Add(orderline);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, ensure CustomerID is correctly populated for the dropdown
            ViewData["CustomerID"] = new SelectList(_context.Users, "Id", "UserName", orderline.CustomerID);
            return View(orderline);
        }
        // GET: ManagerOrderlines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderline = await _context.Orderlines.FindAsync(id);
            if (orderline == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Users, "Id", "Id", orderline.CustomerID);
            return View(orderline);
        }

        // POST: ManagerOrderlines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderlineID,CustomerID,InitialAddress,DestinationAddress,Date,WeightKg,Status")] Orderline orderline)
        {
            if (id != orderline.OrderlineID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderline);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderlineExists(orderline.OrderlineID))
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
            ViewData["CustomerID"] = new SelectList(_context.Users, "Id", "Id", orderline.CustomerID);
            return View(orderline);
        }

        // GET: ManagerOrderlines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderline = await _context.Orderlines
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderlineID == id);
            if (orderline == null)
            {
                return NotFound();
            }

            return View(orderline);
        }

        // POST: ManagerOrderlines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderline = await _context.Orderlines.FindAsync(id);
            if (orderline != null)
            {
                _context.Orderlines.Remove(orderline);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderlineExists(int id)
        {
            return _context.Orderlines.Any(e => e.OrderlineID == id);
        }
    }
}
