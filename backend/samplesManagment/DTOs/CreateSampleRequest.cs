using System.ComponentModel.DataAnnotations;

namespace samplesManagment.DTOs;

public class CreateSampleRequest : IValidatableObject
{
    [Required(ErrorMessage = "El documento del paciente es obligatorio.")]
    [RegularExpression(@"^\d{6,12}$", ErrorMessage = "El documento debe tener solo dígitos y entre 6 y 12 caracteres.")]
    public string? DocumentNumber { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre completo no puede superar 100 caracteres.")]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "El tipo de muestra es obligatorio.")]
    [RegularExpression("^(Sangre|Orina|Heces|Hisopado)$", ErrorMessage = "El tipo de muestra debe ser: Sangre, Orina, Heces o Hisopado.")]
    public string? SampleType { get; set; }

    [Required(ErrorMessage = "La fecha y hora de toma es obligatoria.")]
    public DateTime? CollectedAt { get; set; }

    [Required(ErrorMessage = "La lista de exámenes es obligatoria.")]
    [MinLength(1, ErrorMessage = "Debe enviar al menos un código de examen.")]
    public List<string>? TestCodes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (FullName!.Trim().Length < 3)
            yield return new ValidationResult("El nombre completo debe tener mínimo 3 caracteres.", [nameof(FullName)]);

        if (CollectedAt > DateTime.Now)
            yield return new ValidationResult("La fecha y hora de toma no puede ser futura.", [nameof(CollectedAt)]);

        var codes = TestCodes!.Select(c => c?.Trim() ?? "").ToList();
        if (codes.Any(c => c == ""))
            yield return new ValidationResult("Los códigos de examen no pueden estar vacíos.", [nameof(TestCodes)]);

        var repeated = codes.GroupBy(c => c, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (repeated.Count > 0)
            yield return new ValidationResult($"Códigos de examen repetidos: {string.Join(", ", repeated)}.", [nameof(TestCodes)]);
    }
}
