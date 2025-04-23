using _Kurs_10_Dapper.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace _Kurs_10_Dapper.ViewComponents
{
    public class _MultipleLineChartComponentPartial : ViewComponent
    {
        private readonly ISalesRepository _salesRepository;

        public _MultipleLineChartComponentPartial(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ageGenderdata = await _salesRepository.GetGenderAgeGroupStatsAsync();

            var apiKey = "API KEY";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var ageGenderSummary = string.Join(", ", ageGenderdata.Select(s => $"{s.AgeGroup}: Erkek({s.MaleCount}), Kadın({s.FemaleCount}) - "));


            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are an interpretive statistician." },
                new { role = "user", content = "Bununla alakalı analiz yap ve Türkçe ver bana (maks 500 karakter)(yıllar arası cinsiyete göre müşteri dağılımı):"+ageGenderSummary },
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
                ViewBag.Comment5 = answer;
            }

            else
            {
                Console.WriteLine($"A Failure Occured: {response.StatusCode}");
                Console.WriteLine(responseString);
            }

            return View(ageGenderdata);

        }
    }
}