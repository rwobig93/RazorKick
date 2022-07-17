using Application.Constants;

namespace Application.Extensibility.Extensions;

public static class ApiExtensions
{
    public static void ApiVersionOne(this RouteHandlerBuilder apiMethod) =>
        apiMethod
            .WithApiVersionSet(ApiConstants.SupportsVersionOne!)
            .HasApiVersion(ApiConstants.Version1);
    
    public static void ApiVersionTwo(this RouteHandlerBuilder apiMethod) =>
        apiMethod
            .WithApiVersionSet(ApiConstants.SupportsVersionOne!)
            .HasApiVersion(ApiConstants.Version2);
}