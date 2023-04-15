﻿using System.Net;
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

            response.StatusCode = error switch
            {
                ApiException =>
                    (int) HttpStatusCode.BadRequest,
                KeyNotFoundException =>
                    (int) HttpStatusCode.NotFound,
                _ => (int) HttpStatusCode.InternalServerError
            };
            var result = JsonSerializer.Serialize(responseModel);
            await response.WriteAsync(result);
        }
    }
}