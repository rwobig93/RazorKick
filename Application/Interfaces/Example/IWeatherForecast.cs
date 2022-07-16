using Shared.ApiRequests.Example;
using Shared.ApiResponses.Example;

namespace Application.Interfaces.Example;

public interface IWeatherForecast
{
    Task<WeatherDataResponse[]> GetForecastAsync(WeatherForecastRequest startDate, int count = 100);
}