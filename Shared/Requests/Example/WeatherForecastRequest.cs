using Shared.Models.Base;

namespace Shared.Requests.Example;

public class WeatherForecastRequest : ApiObjectFromQuery<WeatherForecastRequest>
{
    public DateTime StartDate { get; set; } = DateTime.Now;
    public int WeatherCount { get; set; } = 100;
}