using Application.Interfaces.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Infrastructure.Features.Example;

public class WeatherForecast : IWeatherForecast
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest weatherRequest, int count)
    {
        return Task.FromResult(Enumerable.Range(1, count).Select(index => new WeatherDataResponse
        {
            Date = weatherRequest.StartDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray());
    }
}