using Shared.Models.Base;

namespace Shared.ApiRequests.Example;

public class WeatherForecastRequest : ApiObjectFromQuery<WeatherForecastRequest>
{
    public DateTime StartDate { get; set; } = DateTime.Now;
}