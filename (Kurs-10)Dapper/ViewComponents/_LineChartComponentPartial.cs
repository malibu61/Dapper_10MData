using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace _Kurs_10_Dapper.ViewComponents
{
    public class _LineChartComponentPartial : ViewComponent
    {
        private readonly ISalesRepository _salesRepository;

        public _LineChartComponentPartial(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var salesData = await _salesRepository.GetMonthlySalesAsync();

            var apiKey = "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";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var salesSummary = string.Join(", ", salesData.Select(s => $"{s.SalesMonth}: {s.TotalSales}M"));


            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are an interpretive statistician." },
                new { role = "user", content = "Bununla alakalı analiz yap ve iyileştirme için Türkçe fikir ver. (maks 500 karakter)(satışlar milyon cinsinden verilmiştir):"+salesSummary },
            },

                max_tokens = 1000

            };


            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");



            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {

                var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                var answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                ViewBag.Comment1 = answer;
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
