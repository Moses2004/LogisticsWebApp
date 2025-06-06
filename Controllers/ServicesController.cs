using Microsoft.AspNetCore.Mvc;

namespace LogisticsWebApp.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
