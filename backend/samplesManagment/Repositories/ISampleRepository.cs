using samplesManagment.DTOs;

namespace samplesManagment.Repositories;

// Examen del catálogo (con su Id interno)
public class TestRow
{
    public int IdTest { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public interface ISampleRepository
{
    Task<List<TestRow>> GetTestsByCodesAsync(IEnumerable<string> codes);

    // Crea paciente (si no existe), muestra y exámenes en una sola transacción
    Task<int> CreateAsync(int documentNumber, string fullName, string sampleType, DateTime collectedAt, IEnumerable<int> testIds);

    Task<SampleDetail?> GetByIdAsync(int id);

    Task<(List<SampleListItem> Items, int Total)> GetPageAsync(int page, int pageSize);

    // Devuelve false si el estado actual ya no es currentStatus
    Task<bool> UpdateStatusAsync(int id, string currentStatus, string newStatus, string? rejectionReason);
}
