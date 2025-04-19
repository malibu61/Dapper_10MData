using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace _Kurs_10_Dapper.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISalesRepository _salesRepository;

        public SalesController(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var sales = await _salesRepository.GetAllInfoAsync();
            int pageSize = 10;
            var pagedSales = sales.ToPagedList(page, pageSize);

            return View(pagedSales);
        }
    }
}
