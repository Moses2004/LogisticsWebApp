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
    public class LorriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LorriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lorries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lorries.ToListAsync());
        }

        // GET: Lorries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lorry = await _context.Lorries
                .FirstOrDefaultAsync(m => m.LorryID == id);
            if (lorry == null)
            {
                return NotFound();
            }

            return View(lorry);
        }

        // GET: Lorries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lorries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LorryID,LicensePlate,Model,CapacityKg,Status")] Lorry lorry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lorry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lorry);
        }

        

        // POST: Lorries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LorryID,LicensePlate,Model,CapacityKg,Status")] Lorry lorry)
        {
            if (id != lorry.LorryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lorry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LorryExists(lorry.LorryID))
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
            return View(lorry);
        }

        // GET: Lorries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lorry = await _context.Lorries
                .FirstOrDefaultAsync(m => m.LorryID == id);
            if (lorry == null)
            {
                return NotFound();
            }

            return View(lorry);
        }

        // POST: Lorries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lorry = await _context.Lorries.FindAsync(id);
            if (lorry != null)
            {
                _context.Lorries.Remove(lorry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LorryExists(int id)
        {
            return _context.Lorries.Any(e => e.LorryID == id);
        }
    }
}
