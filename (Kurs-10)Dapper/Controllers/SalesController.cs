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

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            var sales = await _salesRepository.GetAllInfoAsync();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();// Küçük-büyük harf duyarsız yapma
                sales = sales.Where(x =>
                    (!string.IsNullOrEmpty(x.NAMESURNAME) && x.NAMESURNAME.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(x.ITEMNAME) && x.ITEMNAME.ToLower().Contains(search))
                ).ToList();
            }

            ViewBag.SearchText = search;

            int pageSize = 10;
            var pagedSales = sales.ToPagedList(page, pageSize);

            return View(pagedSales);
        }
    }
}