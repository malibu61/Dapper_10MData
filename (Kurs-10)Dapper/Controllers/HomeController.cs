using _Kurs_10_Dapper.Models;
using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _Kurs_10_Dapper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISalesRepository _salesRepository;

        public HomeController(ILogger<HomeController> logger, ISalesRepository salesRepository)
        {
            _logger = logger;
            _salesRepository = salesRepository;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _salesRepository.GetAllSalesCount();

            return View(values);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
