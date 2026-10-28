using samplesManagment.DTOs;
using samplesManagment.Exceptions;
using samplesManagment.Repositories;

namespace samplesManagment.Services;

public class SampleService(ISampleRepository repository) : ISampleService
{
    // Estados a los que puede pasar cada estado. Procesada y Rechazada son finales.
    private static readonly Dictionary<string, string[]> Transitions = new()
    {
        ["Registrada"] = ["Recibida", "Rechazada"],
        ["Recibida"] = ["Procesada", "Rechazada"],
        ["Procesada"] = [],
        ["Rechazada"] = [],
    };

    // ---------- Registrar muestra ----------

    public async Task<SampleDetail> CreateAsync(CreateSampleRequest request)
    {
        // 1. Preparar los datos recibidos
        var documentNumber = ConvertDocument(request.DocumentNumber!);
        var fullName = request.FullName!.Trim();
        var codes = request.TestCodes!.Select(code => code.Trim()).ToList();

        // 2. Buscar los exámenes en el catálogo (falla si alguno no existe)
        var tests = await FindTestsAsync(codes);
        var testIds = tests.Select(test => test.IdTest).ToList();

        // 3. Guardar paciente, muestra y exámenes (el repositorio lo hace en una sola transacción)
        var newSampleId = await repository.CreateAsync(
            documentNumber, fullName, request.SampleType!, request.CollectedAt!.Value, testIds);

        // 4. Devolver la muestra creada
        return (await repository.GetByIdAsync(newSampleId))!;
    }

    // En la base el documento es un int; si el número es demasiado grande no cabe
    private static int ConvertDocument(string document)
    {
        if (!int.TryParse(document, out var number))
            throw new BadRequestException("documentNumber",
                "El documento supera el valor máximo que soporta la base de datos (2147483647).");

        return number;
    }

    // Busca los exámenes por código y, si alguno no existe, avisa cuáles son
    private async Task<List<TestRow>> FindTestsAsync(List<string> codes)
    {
        var tests = await repository.GetTestsByCodesAsync(codes);

        var missingCodes = new List<string>();
        foreach (var code in codes)
        {
            var exists = tests.Any(test => test.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (!exists)
                missingCodes.Add(code);
        }

        if (missingCodes.Count > 0)
            throw new BadRequestException("testCodes",
                $"Los siguientes códigos de examen no existen: {string.Join(", ", missingCodes)}.");

        return tests;
    }

    // ---------- Listar muestras ----------

    public async Task<PagedResult<SampleListItem>> GetPageAsync(int page, int pageSize)
    {
        // page y pageSize ya se validaron en el controlador
        var (items, total) = await repository.GetPageAsync(page, pageSize);

        return new PagedResult<SampleListItem>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    // ---------- Cambiar estado ----------

    public async Task<SampleDetail> ChangeStatusAsync(int id, ChangeStatusRequest request)
    {
        // 1. La muestra debe existir
        var sample = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"No existe la muestra con id {id}.");

        // 2. La transición debe estar permitida
        var currentStatus = sample.Status;
        var newStatus = request.Status!;
        var allowedStatuses = Transitions[currentStatus];

        if (!allowedStatuses.Contains(newStatus))
            throw new ConflictException($"No se puede cambiar el estado de '{currentStatus}' a '{newStatus}'.");

        // 3. El motivo solo se guarda cuando se rechaza (el DTO ya comprobó que viene)
        string? rejectionReason = null;
        if (newStatus == "Rechazada")
            rejectionReason = request.RejectionReason!.Trim();

        // 4. Actualizar. Devuelve false si otro usuario cambió el estado mientras tanto.
        var updated = await repository.UpdateStatusAsync(id, currentStatus, newStatus, rejectionReason);
        if (!updated)
            throw new ConflictException("El estado de la muestra cambió mientras se procesaba la solicitud. Intente de nuevo.");

        // 5. Devolver la muestra actualizada
        return (await repository.GetByIdAsync(id))!;
    }
}
