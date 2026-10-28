using samplesManagment.DTOs;
using samplesManagment.Exceptions;

namespace samplesManagment.Middleware;

// Único lugar que convierte excepciones en respuestas de error
public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, body) = ex switch
            {
                BadRequestException e => (400, new ErrorResponse(400, "Hay errores de validación.",
                    new() { [e.Field] = [e.Message] })),
                NotFoundException e => (404, new ErrorResponse(404, e.Message)),
                ConflictException e => (409, new ErrorResponse(409, e.Message)),
                _ => (500, new ErrorResponse(500, "Ocurrió un error inesperado. Intente de nuevo más tarde.")),
            };

            // El detalle completo solo va al log, nunca al cliente
            if (status == 500)
                logger.LogError(ex, "Error inesperado en {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(body);
        }
    }
}
