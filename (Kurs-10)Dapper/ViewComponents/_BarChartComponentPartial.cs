using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _Kurs_10_Dapper.ViewComponents
{
    public class _BarChartComponentPartial : ViewComponent
    {
        private readonly ISalesRepository _salesRepository;

        public _BarChartComponentPartial(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var salesData = await _salesRepository.GetSalesDataByBrand(); // Veriyi al

            var apiKey = "API KEY";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var top10Brand = string.Join(", ", salesData.Select(s => $"{s.Brand}: {s.TotalSold}"));


            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are an interpretive statistician. " },
                new { role = "user", content = "Bununla alakalı analiz yap ve iyileştirme için fikir ver.Cevabı Türkçe ver (maks 500 karakter)(Top10 Markaya ait satışlar bunlar):"+top10Brand },
            },

                max_tokens = 1000// dönecek olan cevabın karakter uzunluğu

            };


            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {

                var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                var answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                ViewBag.Comment2 = answer;
            }

            else
            {
                Console.WriteLine($"A Failure Occured: {response.StatusCode}");
                Console.WriteLine(responseString);
            }

            return View(salesData);

        }
    }
}