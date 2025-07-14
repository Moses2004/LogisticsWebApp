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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using LogisticsWebApp.Models.ViewModels;
using ClosedXML.Excel; 
using System.IO;     

namespace LogisticsWebApp.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InvoicesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            IQueryable<Invoice> invoicesQuery = _context.Invoices
                .Include(i => i.Orderline)
                .Include(i => i.TransportUnit)
                .Include(i => i.Customer);

            if (User.IsInRole("Manager"))
            {
                return View(await invoicesQuery.ToListAsync());
            }
            else if (User.IsInRole("Client"))
            {
                return View(await invoicesQuery.Where(i => i.CustomerId == currentUserId).ToListAsync());
            }
            else
            {
                return Forbid();
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Orderline)
                .Include(i => i.TransportUnit)
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(m => m.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Manager"))
            {
                return View(invoice);
            }
            else if (User.IsInRole("Client"))
            {
                var currentUserId = _userManager.GetUserId(User);
                if (invoice.CustomerId == currentUserId)
                {
                    return View(invoice);
                }
                else
                {
                    return Forbid();
                }
            }
            else
            {
                return Forbid();
            }
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID");
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container");
            ViewData["CustomerId"] = new SelectList(_userManager.Users, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([Bind("InvoiceID,OrderlineID,TransportUnitID,TotalAmount,PaymentStatus,CustomerId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID", invoice.OrderlineID);
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container", invoice.TransportUnitID);
            ViewData["CustomerId"] = new SelectList(_userManager.Users, "Id", "Email", invoice.CustomerId);
            return View(invoice);
        }

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
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID", invoice.OrderlineID);
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container", invoice.TransportUnitID);
            return View(invoice);
        }

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
            ViewData["OrderlineID"] = new SelectList(_context.Orderlines, "OrderlineID", "OrderlineID", invoice.OrderlineID);
            ViewData["TransportUnitID"] = new SelectList(_context.TransportUnits, "TransportUnitID", "Container", invoice.TransportUnitID);
            return View(invoice);
        }

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
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

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

        // GET: Invoices/Report - Only Managers can access
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Report()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Customer)
                .ToListAsync();

            var viewModel = new InvoiceReportViewModel
            {
                TotalInvoices = invoices.Count,
                TotalRevenue = invoices.Sum(i => i.TotalAmount)
            };

            viewModel.InvoicesByStatus = invoices
                .GroupBy(i => i.PaymentStatus)
                .ToDictionary(g => g.Key, g => g.Count());

            viewModel.RevenueByStatus = invoices
                .GroupBy(i => i.PaymentStatus)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

            viewModel.InvoicesByCustomer = invoices
                .GroupBy(i => i.Customer != null ? i.Customer.Email : "Unknown Customer")
                .ToDictionary(g => g.Key, g => g.Count());

            viewModel.RevenueByCustomer = invoices
                .GroupBy(i => i.Customer != null ? i.Customer.Email : "Unknown Customer")
                .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

            viewModel.RevenueByMonth = invoices
                .Where(i => i.InvoiceDate.HasValue)
                .GroupBy(i => i.InvoiceDate.Value.ToString("MMM yyyy"))
                .OrderBy(g => g.Min(i => i.InvoiceDate.Value))
                .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

            return View(viewModel);
        }

        // New Action: Export Report to Excel
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ExportToExcel()
        {
            // Retrieve all invoices (similar to your Report action, but for export)
            var invoices = await _context.Invoices
                .Include(i => i.Orderline)
                .Include(i => i.TransportUnit)
                .Include(i => i.Customer)
                .ToListAsync();

            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet for "All Invoices"
                var worksheetAllInvoices = workbook.Worksheets.Add("All Invoices");
                worksheetAllInvoices.Cell(1, 1).Value = "Invoice Report - All Invoices";
                worksheetAllInvoices.Cell(2, 1).Value = "Export Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                // Add headers for detailed invoice data
                worksheetAllInvoices.Cell(4, 1).Value = "Invoice ID";
                worksheetAllInvoices.Cell(4, 2).Value = "Customer Email";
                worksheetAllInvoices.Cell(4, 3).Value = "Total Amount";
                worksheetAllInvoices.Cell(4, 4).Value = "Payment Status";
                worksheetAllInvoices.Cell(4, 5).Value = "Invoice Date";
                worksheetAllInvoices.Cell(4, 6).Value = "Orderline ID";
                worksheetAllInvoices.Cell(4, 7).Value = "Transport Unit";

                // Populate data rows for all invoices
                int currentRow = 5;
                foreach (var invoice in invoices)
                {
                    worksheetAllInvoices.Cell(currentRow, 1).Value = invoice.InvoiceID;
                    worksheetAllInvoices.Cell(currentRow, 2).Value = invoice.Customer?.Email ?? "N/A";
                    worksheetAllInvoices.Cell(currentRow, 3).Value = invoice.TotalAmount;
                    worksheetAllInvoices.Cell(currentRow, 4).Value = invoice.PaymentStatus;
                    worksheetAllInvoices.Cell(currentRow, 5).Value = invoice.InvoiceDate?.ToString("yyyy-MM-dd") ?? "N/A";
                    worksheetAllInvoices.Cell(currentRow, 6).Value = invoice.OrderlineID;
                    worksheetAllInvoices.Cell(currentRow, 7).Value = invoice.TransportUnit?.Container ?? "N/A";
                    currentRow++;
                }

                // Auto-fit columns for the "All Invoices" sheet
                worksheetAllInvoices.Columns().AdjustToContents();

                // Add a worksheet for "Summary by Status" (similar to your ViewModel data)
                var worksheetStatusSummary = workbook.Worksheets.Add("Summary by Status");
                worksheetStatusSummary.Cell(1, 1).Value = "Summary - Invoices by Payment Status";
                worksheetStatusSummary.Cell(3, 1).Value = "Payment Status";
                worksheetStatusSummary.Cell(3, 2).Value = "Number of Invoices";
                worksheetStatusSummary.Cell(3, 3).Value = "Total Revenue";

                int statusRow = 4;
                var invoicesByStatus = invoices.GroupBy(i => i.PaymentStatus).ToDictionary(g => g.Key, g => g.Count());
                var revenueByStatus = invoices.GroupBy(i => i.PaymentStatus).ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

                foreach (var status in invoicesByStatus.Keys)
                {
                    worksheetStatusSummary.Cell(statusRow, 1).Value = status;
                    worksheetStatusSummary.Cell(statusRow, 2).Value = invoicesByStatus[status];
                    worksheetStatusSummary.Cell(statusRow, 3).Value = revenueByStatus[status];
                    statusRow++;
                }
                worksheetStatusSummary.Columns().AdjustToContents();

                // Add a worksheet for "Summary by Customer"
                var worksheetCustomerSummary = workbook.Worksheets.Add("Summary by Customer");
                worksheetCustomerSummary.Cell(1, 1).Value = "Summary - Invoices by Customer";
                worksheetCustomerSummary.Cell(3, 1).Value = "Customer Email";
                worksheetCustomerSummary.Cell(3, 2).Value = "Number of Invoices";
                worksheetCustomerSummary.Cell(3, 3).Value = "Total Revenue";

                int customerRow = 4;
                var invoicesByCustomer = invoices.GroupBy(i => i.Customer != null ? i.Customer.Email : "Unknown Customer").ToDictionary(g => g.Key, g => g.Count());
                var revenueByCustomer = invoices.GroupBy(i => i.Customer != null ? i.Customer.Email : "Unknown Customer").ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

                foreach (var customer in invoicesByCustomer.Keys)
                {
                    worksheetCustomerSummary.Cell(customerRow, 1).Value = customer;
                    worksheetCustomerSummary.Cell(customerRow, 2).Value = invoicesByCustomer[customer];
                    worksheetCustomerSummary.Cell(customerRow, 3).Value = revenueByCustomer[customer];
                    customerRow++;
                }
                worksheetCustomerSummary.Columns().AdjustToContents();

                // Add a worksheet for "Revenue by Month"
                var worksheetRevenueByMonth = workbook.Worksheets.Add("Revenue by Month");
                worksheetRevenueByMonth.Cell(1, 1).Value = "Summary - Revenue by Month";
                worksheetRevenueByMonth.Cell(3, 1).Value = "Month";
                worksheetRevenueByMonth.Cell(3, 2).Value = "Total Revenue";

                int monthRow = 4;
                var revenueByMonth = invoices
                    .Where(i => i.InvoiceDate.HasValue)
                    .GroupBy(i => i.InvoiceDate.Value.ToString("MMM yyyy"))
                    .OrderBy(g => g.Min(i => i.InvoiceDate.Value))
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

                foreach (var month in revenueByMonth.Keys)
                {
                    worksheetRevenueByMonth.Cell(monthRow, 1).Value = month;
                    worksheetRevenueByMonth.Cell(monthRow, 2).Value = revenueByMonth[month];
                    monthRow++;
                }
                worksheetRevenueByMonth.Columns().AdjustToContents();


                // Prepare the workbook for download
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InvoiceReport.xlsx");
                }
            }
        }
    }
}