using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace _Kurs_10_Dapper.ViewComponents
{
    public class _BrandPreferredInfoComponentPartial : ViewComponent
    {
        private readonly ISalesRepository _salesRepository;


        public _BrandPreferredInfoComponentPartial(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _salesRepository.BrandPreferenceInfoAsync();
            return View(values);
        }
    }
}