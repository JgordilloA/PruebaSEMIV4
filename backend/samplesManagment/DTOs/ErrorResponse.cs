using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace samplesManagment.DTOs;

// Estructura única para todas las respuestas de error
public class ErrorResponse(int status, string message, Dictionary<string, string[]>? errors = null)
{
    public int Status { get; } = status;
    public string Message { get; } = message;
    // Mensajes por campo (vacío si el error no es de validación)
    public Dictionary<string, string[]> Errors { get; } = errors ?? new();

    // Convierte los errores de validación de los DTOs a esta estructura (campos en camelCase)
    public static ErrorResponse FromModelState(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(e => e.Value!.Errors.Count > 0)
            .ToDictionary(
                e => e.Key.Length == 0 ? e.Key : char.ToLowerInvariant(e.Key[0]) + e.Key[1..],
                e => e.Value!.Errors
                    .Select(x => string.IsNullOrEmpty(x.ErrorMessage) ? "Valor no válido." : x.ErrorMessage)
                    .ToArray());
        return new ErrorResponse(400, "Hay errores de validación.", errors);
    }
}