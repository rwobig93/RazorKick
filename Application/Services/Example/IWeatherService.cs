using Application.Requests.Example;
using Application.Responses.Example;

namespace Application.Services.Example;

public interface IWeatherService
{
    Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest startDate, int count = 100);
}