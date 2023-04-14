using System.Net;
using System.Text.Json;
using Application.Models.Web;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            _logger.Error("Error occurred and handled by the middleware: {ErrorMessage}", error.Message);
            
            var response = context.Response;
            response.ContentType = "application/json";
            var responseModel = await Result<string>.FailAsync(error.Message);

            switch (error)
            {
                case ApiException:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
            var result = JsonSerializer.Serialize(responseModel);
            await response.WriteAsync(result);
        }
    }
}