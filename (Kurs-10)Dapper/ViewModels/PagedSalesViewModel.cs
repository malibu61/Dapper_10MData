namespace _Kurs_10_Dapper.ViewModels
{
    public class PagedSalesViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<GetAllinfoViewModel> Items { get; set; }

    }
}
