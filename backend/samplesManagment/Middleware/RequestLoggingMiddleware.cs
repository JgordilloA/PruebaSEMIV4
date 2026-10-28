using System.Diagnostics;

namespace samplesManagment.Middleware;

// Imprime en consola: método, ruta, código de estado y milisegundos
public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
     Console.WriteLine($"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode}");
     await next(context);

    }
}
