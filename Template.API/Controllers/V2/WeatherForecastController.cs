using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Template.API.Controllers.V2;

[ApiVersion("2.0")]
[ApiController]
[Route(BaseUrl)]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : BaseController
{
    private static readonly string[] s_summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = s_summaries[Random.Shared.Next(s_summaries.Length)]
        })
        .ToArray();
    }
}
