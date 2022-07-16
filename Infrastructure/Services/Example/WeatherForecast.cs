using Application.Interfaces.Example;
using Shared.ApiRequests.Example;
using Shared.ApiResponses.Example;

namespace Infrastructure.Services.Example;

public class WeatherForecast : IWeatherForecast
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest weatherRequest)
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherDataResponse
        {
            Date = weatherRequest.StartDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray());
    }
}