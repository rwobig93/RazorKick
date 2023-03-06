namespace Application.Constants.Web;

public static class AppRouteConstants
{
    public const string Index = "/";
    
    public static class Example
    {
        public const string Counter = "/counter";
        public const string WeatherData = "/weather-data";
        public const string Books = "/books";
    }
    
    public static class Identity
    {
        public const string Login = "/login";
        public const string Register = "/register";
        public const string ConfirmEmail = "/confirm-email";
    }

    public static class Api
    {
        public const string Root = "/api";
    }

    public static class Jobs
    {
        public const string Root = "/jobs";
    }
}