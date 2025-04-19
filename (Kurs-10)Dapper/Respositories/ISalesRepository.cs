using _Kurs_10_Dapper.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace _Kurs_10_Dapper.Respositories
{
    public interface ISalesRepository
    {
        Task<int> GetAllSalesCount();
        Task<List<MonthlySalesViewModel>> GetMonthlySalesAsync();
        Task<List<BrandSalesViewModel>> GetSalesDataByBrand();
        Task<List<AgeRangeViewModel>> GetUserCountByAgeRangeAsync();
        Task<List<CityOrderViewModel>> GetTop10CitiesByOrderAsync();
        Task<List<GenderAgeGroupViewModel>> GetGenderAgeGroupStatsAsync();
        Task<List<GetAllinfoViewModel>> GetAllInfoAsync();
        Task<List<BrandPreferenceViewModel>> BrandPreferenceInfoAsync();

    }
}
