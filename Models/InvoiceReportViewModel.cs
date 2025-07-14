// Models/ViewModels/InvoiceReportViewModel.cs (or create a new 'ViewModels' folder)
using System.Collections.Generic;

namespace LogisticsWebApp.Models.ViewModels
{
    public class InvoiceReportViewModel
    {
        public int TotalInvoices { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> InvoicesByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByStatus { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> InvoicesByCustomer { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByCustomer { get; set; } = new Dictionary<string, decimal>();

        public Dictionary<string, decimal> RevenueByMonth { get; set; } = new Dictionary<string, decimal>();
    }
}