using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace _Kurs_10_Dapper.ViewComponents
{
    public class _AllinfoComponentPartial : ViewComponent
    {
        private readonly ISalesRepository _salesRepository;


        public _AllinfoComponentPartial(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(int page=1)
        {
            //var sales = await _salesRepository.GetAllInfoAsync();
            //int pageSize = 10;
            //var pagedSales = sales.ToPagedList(page, pageSize);

            return View(/*pagedSales*/);
        }
    }
}