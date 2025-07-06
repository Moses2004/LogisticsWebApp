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
    public class TransportUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransportUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TransportUnits
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TransportUnits.Include(t => t.Assistant).Include(t => t.Driver).Include(t => t.Lorry);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TransportUnits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportUnit = await _context.TransportUnits
                .Include(t => t.Assistant)
                .Include(t => t.Driver)
                .Include(t => t.Lorry)
                .FirstOrDefaultAsync(m => m.TransportUnitID == id);
            if (transportUnit == null)
            {
                return NotFound();
            }

            return View(transportUnit);
        }

        // GET: TransportUnits/Create
        public IActionResult Create()
        {
            // Populate ViewBag with SelectLists for dropdowns
            ViewData["LorryID"] = new SelectList(_context.Lorries, "LorryID", "LicensePlate");
            ViewData["DriverID"] = new SelectList(_context.Drivers, "DriverID", "Name");
            ViewData["AssistantID"] = new SelectList(_context.Assistants, "AssistantID", "Name");
            return View();
        }

        // POST: TransportUnits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
           // **IMPORTANT: Update the [Bind] attribute below**
           // Include only the scalar properties and Foreign Key IDs from your TransportUnit model
           // DO NOT include the navigation properties (e.g., Lorry, Driver, Assistant)
           [Bind("TransportUnitID,LorryID,DriverID,AssistantID,Container,Status")] TransportUnit transportUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transportUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect to the Index page on success
            }

            // If ModelState.IsValid is false (validation failed on server-side),
            // you must re-populate the ViewData for dropdowns before returning the view
            // so that the dropdowns aren't empty when the form reloads with validation errors.
            ViewData["LorryID"] = new SelectList(_context.Lorries, "LorryID", "LicensePlate", transportUnit.LorryID);
            ViewData["DriverID"] = new SelectList(_context.Drivers, "DriverID", "Name", transportUnit.DriverID);
            ViewData["AssistantID"] = new SelectList(_context.Assistants, "AssistantID", "Name", transportUnit.AssistantID);
            // If "Status" is a dropdown, re-populate it here with the selected value:
            // ViewData["Status"] = new SelectList(new List<string> { "Available", "In Transit", "Maintenance", "Completed" }, transportUnit.Status);

            return View(transportUnit); // Return the view with the model and validation errors
        }


        // GET: TransportUnits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportUnit = await _context.TransportUnits.FindAsync(id);
            if (transportUnit == null)
            {
                return NotFound();
            }
            ViewData["AssistantID"] = new SelectList(_context.Assistants, "AssistantID", "Name", transportUnit.AssistantID);
            ViewData["DriverID"] = new SelectList(_context.Drivers, "DriverID", "Name", transportUnit.DriverID);
            ViewData["LorryID"] = new SelectList(_context.Lorries, "LorryID", "LicensePlate", transportUnit.LorryID);
            return View(transportUnit);
        }

        // POST: TransportUnits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransportUnitID,LorryID,DriverID,AssistantID,Container,Status")] TransportUnit transportUnit)
        {
            if (id != transportUnit.TransportUnitID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transportUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransportUnitExists(transportUnit.TransportUnitID))
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
            ViewData["AssistantID"] = new SelectList(_context.Assistants, "AssistantID", "Name", transportUnit.AssistantID);
            ViewData["DriverID"] = new SelectList(_context.Drivers, "DriverID", "Name", transportUnit.DriverID);
            ViewData["LorryID"] = new SelectList(_context.Lorries, "LorryID", "LicensePlate", transportUnit.LorryID);
            return View(transportUnit);
        }

        // GET: TransportUnits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transportUnit = await _context.TransportUnits
                .Include(t => t.Assistant)
                .Include(t => t.Driver)
                .Include(t => t.Lorry)
                .FirstOrDefaultAsync(m => m.TransportUnitID == id);
            if (transportUnit == null)
            {
                return NotFound();
            }

            return View(transportUnit);
        }

        // POST: TransportUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transportUnit = await _context.TransportUnits.FindAsync(id);
            if (transportUnit != null)
            {
                _context.TransportUnits.Remove(transportUnit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransportUnitExists(int id)
        {
            return _context.TransportUnits.Any(e => e.TransportUnitID == id);
        }
    }
}
