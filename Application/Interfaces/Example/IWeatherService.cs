using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Interfaces.Example;

public interface IWeatherService
{
    Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest startDate, int count = 100);
}