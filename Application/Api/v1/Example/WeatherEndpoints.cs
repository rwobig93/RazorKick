using Application.Helpers.Web;
using Application.Models.Web;
using Application.Requests.Example;
using Application.Responses.Example;
using Application.Services.Example;
using Microsoft.AspNetCore.Mvc;

namespace Application.Api.v1.Example;

public static class WeatherEndpoints
{
    public static void MapEndpointsWeather(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/weather", GetForecastAsync).ApiVersionOne();
    }

    private static async Task<IResult<WeatherDataResponse[]>> GetForecastAsync([FromQuery]WeatherForecastRequest? weatherRequest, IWeatherService 
    weatherForecast)
    {
        try
        {
            weatherRequest ??= new WeatherForecastRequest() {StartDate = DateOnly.FromDateTime(DateTime.Now)};

            var weatherForecastData = await weatherForecast.GetForecastAsync(weatherRequest);
            
            return await Result<WeatherDataResponse[]>.SuccessAsync(weatherForecastData);
        }
        catch (Exception ex)
        {
            return await Result<WeatherDataResponse[]>.FailAsync(ex.Message);
        }
    }
}