namespace Shared.ApiRoutes;

public static class ApiRouteExtensions
{
    public static string ToFullUrl(this string uri, string hostOrigin) =>
        string.Concat(hostOrigin, uri);
}