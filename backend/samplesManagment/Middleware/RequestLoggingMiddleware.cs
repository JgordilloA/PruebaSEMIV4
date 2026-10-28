using System.Diagnostics;

namespace samplesManagment.Middleware;

// Imprime en consola: método, ruta, código de estado de la respuesta y milisegundos
public class RequestLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Se procesa la solicitud; el código de estado solo se conoce cuando termina
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            Console.WriteLine($"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode} {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
