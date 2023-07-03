using Application.Models.Web;

namespace Application.Requests.Example;

public class WeatherForecastRequest : ApiObjectFromQuery<WeatherForecastRequest>
{
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int WeatherCount { get; set; } = 100;
}