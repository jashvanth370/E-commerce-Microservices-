using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // default variables
            string message = "Sorry, Internal Server Error occured Try again later.";
            int statusCode = (int)StatusCodes.Status500InternalServerError;
            string title = "Error";
            try
            {
                await next(context);
                // check if the response status code indicates an error // 429
                if(context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many requests. Please try again later.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await modifyHeader(context, title, message, statusCode);
                }

                // check if the response status code indicates an error // 401
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await modifyHeader(context, title, message, statusCode);
                }

                // check if the response status code indicate an error // 403
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of access";
                    message = "You are not allowed/required to access.";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await modifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Log original Exceptions /File /Debugger /Console
                LogException.LogExceptions(ex);

                // check if excepton timeout
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Time is out";
                    message = "Request timeout... try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                //if none of above exceptions || Exception caugth then do defaoult
                await modifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task modifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // dispaly scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }),CancellationToken.None);
            return;
        }
    }
}
