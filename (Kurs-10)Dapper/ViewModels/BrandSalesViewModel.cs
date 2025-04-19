namespace _Kurs_10_Dapper.ViewModels
{
    public class BrandSalesViewModel
    {
        public string Brand { get; set; }              // Marka adı
        public int TotalSold { get; set; }             // Formatlanmamış değer (ilk hanesi)
        public string Unit { get; set; }               // M, B, K gibi birim
    }
}
