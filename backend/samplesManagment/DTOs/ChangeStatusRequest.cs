using System.ComponentModel.DataAnnotations;

namespace samplesManagment.DTOs;

public class ChangeStatusRequest : IValidatableObject
{
    [Required(ErrorMessage = "El nuevo estado es obligatorio.")]
    [RegularExpression("^(Registrada|Recibida|Procesada|Rechazada)$", ErrorMessage = "El estado debe ser: Registrada, Recibida, Procesada o Rechazada.")]
    public string? Status { get; set; }

    [MaxLength(200, ErrorMessage = "El motivo de rechazo no puede superar 200 caracteres.")]
    public string? RejectionReason { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Status == "Rechazada" && string.IsNullOrWhiteSpace(RejectionReason))
            yield return new ValidationResult("El motivo de rechazo es obligatorio cuando el estado es Rechazada.", [nameof(RejectionReason)]);
    }
}
