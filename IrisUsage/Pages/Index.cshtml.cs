using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using IrisUsage;

namespace IrisUsage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;

        public IndexModel(ILogger<IndexModel> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        [BindProperty] public float SepalLength { get; set; }
        [BindProperty] public float SepalWidth { get; set; }
        [BindProperty] public float PetalLength { get; set; }
        [BindProperty] public float PetalWidth { get; set; }

        public IrisResult? Prediction { get; set; }

        public string? ApiResponse { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            var requestData = new Iris
            {
                SepalLength = SepalLength,
                SepalWidth = SepalWidth,
                PetalLength = PetalLength,
                PetalWidth = PetalWidth
            };

            var json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // bypass SSL validation in dev
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            };
            using var client = new HttpClient(handler);

            var response = await _httpClient.PostAsync("https://localhost:7107/iris", content);

            ApiResponse = await response.Content.ReadAsStringAsync();

            // Deserialize into IrisResult
            Prediction = JsonSerializer.Deserialize<IrisResult>(
                ApiResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return Page();
        }
    }
}
