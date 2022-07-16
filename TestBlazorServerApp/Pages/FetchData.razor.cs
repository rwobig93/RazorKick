using Shared.ApiRequests.Example;
using Shared.ApiResponses.Example;

namespace TestBlazorServerApp.Pages;

public partial class FetchData
{
    private bool _dense = true;
    private bool _hover = true;
    private bool _striped = true;
    private bool _bordered;
    private string _searchString = "";
    private WeatherDataResponse _selectedItem = null!;
    
    private WeatherDataResponse[]? _forecasts;

    protected override async Task OnInitializedAsync()
    {
        _forecasts = await WeatherForecast.GetForecastAsync(new WeatherForecastRequest());
    }

    private bool SearchFunction(WeatherDataResponse weatherResponse)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        if ($"{weatherResponse.Date} {weatherResponse.Summary}".Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return $"{weatherResponse.TemperatureC} {weatherResponse.TemperatureF}".Contains(_searchString, 
            StringComparison.OrdinalIgnoreCase);
    }
}