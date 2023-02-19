using Application.Constants.Web;

namespace Application.Helpers.Web;

public static class ApiHelpers
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