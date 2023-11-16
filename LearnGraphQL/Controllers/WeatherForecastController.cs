using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client;
using System.Net.Http.Headers;

namespace LearnGraphQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly OpenIddictClientService _openIddictClientService;
        private readonly HttpClient _httClient;



        public WeatherForecastController(ILogger<WeatherForecastController> logger, OpenIddictClientService openIddictClientService, HttpClient httClient)
        {
            _logger = logger;
            _openIddictClientService = openIddictClientService;
            _httClient = httClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var tokenResult = await _openIddictClientService.AuthenticateWithClientCredentialsAsync(new Uri("https://localhost:7268/", UriKind.Absolute));

            var token = tokenResult.Response.AccessToken;

            using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7164/WeatherForecast");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}