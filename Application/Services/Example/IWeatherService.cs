using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Services.Example;

public interface IWeatherService
{
    Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest startDate, int count = 100);
}