using Application.Constants.Web;
using Application.Helpers.Web;
using Application.Models.Web;
using Application.Responses.Example;
using Application.Services.Example;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Application.Api.v1.Example;

public static class WeatherEndpoints
{
    public static void MapEndpointsWeather(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRouteConstants.Example.Weather, GetForecastAsync).ApiVersionOne();
    }

    [AllowAnonymous]
    private static async Task<IResult<WeatherDataResponse[]>> GetForecastAsync([FromQuery]DateOnly? startDate, [FromQuery]int? weatherCount,
        IWeatherService  weatherForecast)
    {
        try
        {
            var startDateConverted = startDate ?? DateOnly.FromDateTime(DateTime.Now);
            var weatherCountConverted = weatherCount ?? 10;

            var weatherForecastData = await weatherForecast.GetForecastAsync(startDateConverted, weatherCountConverted);
            
            return await Result<WeatherDataResponse[]>.SuccessAsync(weatherForecastData);
        }
        catch (Exception ex)
        {
            return await Result<WeatherDataResponse[]>.FailAsync(ex.Message);
        }
    }
}