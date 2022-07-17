using Application.Interfaces.Example;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.Example;

namespace Application.Api.v1.Example;

public static class WeatherEndpoints
{
    public static void MapEndpointsWeather(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/weather", GetForecastAsync).ApiVersionOne();
    }

    private static async Task<IResult> GetForecastAsync([FromQuery]WeatherForecastRequest? weatherRequest, IWeatherForecast weatherForecast)
    {
        try
        {
            weatherRequest ??= new WeatherForecastRequest() {StartDate = DateTime.Now};
            return Results.Ok(await weatherForecast.GetForecastAsync(weatherRequest));
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}