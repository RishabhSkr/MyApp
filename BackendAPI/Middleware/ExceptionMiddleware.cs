using System.Net;
using System.Text.Json;
using BackendAPI.Exceptions;

namespace BackendAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {   
            // Console.WriteLine(ex.Message);
            var statusCode = ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                ForbiddenException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.InternalServerError
                
            };
            var msg = ex switch
            {
                NotFoundException => ex.Message,
                BadRequestException => ex.Message,
                UnauthorizedException => ex.Message,
                ForbiddenException => ex.Message,
                _ => "Something went wrong"
            };
            await HandleException(context, HttpStatusCode.InternalServerError, msg);
        }
    }

    private static async Task HandleException(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = context.Response.StatusCode,
            message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
