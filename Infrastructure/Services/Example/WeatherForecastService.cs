using Application.Services.Weather;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Infrastructure.Services.Example;

public class WeatherForecastService : IWeatherService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public async Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest startDate, int count = 100)
    {
        return await Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherDataResponse
        {
            Date = startDate.StartDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToArray());
    }
}