using Dapper;
using Microsoft.Data.SqlClient;
using samplesManagment.DTOs;

namespace samplesManagment.Repositories;

public class SampleRepository(IConfiguration config) : ISampleRepository
{
    // La cadena de conexión viene de appsettings.json
    private readonly string _connectionString =
        config.GetConnectionString("Default") ?? throw new InvalidOperationException("Falta ConnectionStrings:Default.");

    private SqlConnection NewConnection() => new(_connectionString);

    // ---------- Catálogo de exámenes ----------

    public async Task<List<TestRow>> GetTestsByCodesAsync(IEnumerable<string> codes)
    {
        const string sql = @"
            SELECT IdTest, CodeTest AS Code, NameTest AS Name
            FROM Test
            WHERE CodeTest IN @codes";

        using var connection = NewConnection();
        var tests = await connection.QueryAsync<TestRow>(sql, new { codes });
        return tests.ToList();
    }

    // ---------- Registrar muestra ----------

    public async Task<int> CreateAsync(int documentNumber, string fullName, string sampleType, DateTime collectedAt, IEnumerable<int> testIds)
    {
        using var connection = NewConnection();
        await connection.OpenAsync();

        // Todo se hace en una sola transacción: si algo falla antes del Commit,
        // al salir se deshace todo y no queda información parcial
        using var transaction = connection.BeginTransaction();

        // 1. Paciente: usar el que ya existe o crearlo
        var patientId = await FindPatientIdAsync(connection, transaction, documentNumber)
                        ?? await InsertPatientAsync(connection, transaction, documentNumber, fullName);

        // 2. Muestra nueva (siempre empieza en estado Registrada)
        var sampleId = await InsertSampleAsync(connection, transaction, patientId, sampleType, collectedAt);

        // 3. Exámenes de la muestra
        await InsertSampleTestsAsync(connection, transaction, sampleId, testIds);

        await transaction.CommitAsync();
        return sampleId;
    }

    // UPDLOCK + HOLDLOCK bloquean la fila hasta terminar la transacción,
    // así dos solicitudes simultáneas con el mismo documento no crean pacientes duplicados
    private static Task<int?> FindPatientIdAsync(SqlConnection connection, SqlTransaction transaction, int documentNumber)
    {
        const string sql = @"
            SELECT IdPatient
            FROM Patient WITH (UPDLOCK, HOLDLOCK)
            WHERE DocumentNumber = @documentNumber";

        return connection.QuerySingleOrDefaultAsync<int?>(sql, new { documentNumber }, transaction);
    }

    private static Task<int> InsertPatientAsync(SqlConnection connection, SqlTransaction transaction, int documentNumber, string fullName)
    {
        const string sql = @"
            INSERT INTO Patient (DocumentNumber, FullName, CreatedAt)
            OUTPUT INSERTED.IdPatient
            VALUES (@documentNumber, @fullName, GETDATE())";

        return connection.QuerySingleAsync<int>(sql, new { documentNumber, fullName }, transaction);
    }

    private static Task<int> InsertSampleAsync(SqlConnection connection, SqlTransaction transaction, int patientId, string sampleType, DateTime collectedAt)
    {
        const string sql = @"
            INSERT INTO Samples (PatientId, SampleType, CollectedAt, Status, CreatedAt)
            OUTPUT INSERTED.IdSample
            VALUES (@patientId, @sampleType, @collectedAt, 'Registrada', GETDATE())";

        return connection.QuerySingleAsync<int>(sql, new { patientId, sampleType, collectedAt }, transaction);
    }

    // Al pasarle una lista de parámetros, Dapper ejecuta el INSERT una vez por cada examen
    private static Task<int> InsertSampleTestsAsync(SqlConnection connection, SqlTransaction transaction, int sampleId, IEnumerable<int> testIds)
    {
        const string sql = "INSERT INTO SampleTest (SampleId, TestId) VALUES (@sampleId, @testId)";

        var rows = testIds.Select(testId => new { sampleId, testId });
        return connection.ExecuteAsync(sql, rows, transaction);
    }

    // ---------- Consultar una muestra ----------

    public async Task<SampleDetail?> GetByIdAsync(int id)
    {
        // Dos consultas en un solo viaje a la base: primero la muestra, luego sus exámenes
        const string sql = @"
            SELECT s.IdSample AS Id,
                   CAST(p.DocumentNumber AS nvarchar(12)) AS DocumentNumber,
                   p.FullName,
                   s.SampleType, s.CollectedAt, s.Status, s.RejectionReason
            FROM Samples s
            JOIN Patient p ON p.IdPatient = s.PatientId
            WHERE s.IdSample = @id;

            SELECT t.CodeTest AS Code, t.NameTest AS Name
            FROM SampleTest st
            JOIN Test t ON t.IdTest = st.TestId
            WHERE st.SampleId = @id
            ORDER BY t.CodeTest;";

        using var connection = NewConnection();
        using var results = await connection.QueryMultipleAsync(sql, new { id });

        var sample = await results.ReadFirstOrDefaultAsync<SampleDetail>();
        if (sample != null)
        {
            var tests = await results.ReadAsync<TestItem>();
            sample.Tests = tests.ToList();
        }

        return sample;
    }

    // ---------- Listar muestras ----------

    public async Task<(List<SampleListItem> Items, int Total)> GetPageAsync(int page, int pageSize)
    {
        // La paginación se hace en SQL con OFFSET/FETCH (no en memoria).
        // Se ordena también por Id para que fechas iguales no cambien el orden entre páginas.
        const string sql = @"
            SELECT s.IdSample AS Id,
                   CAST(p.DocumentNumber AS nvarchar(12)) AS DocumentNumber,
                   p.FullName AS PatientName,
                   s.SampleType, s.CollectedAt, s.Status,
                   (SELECT COUNT(*) FROM SampleTest st WHERE st.SampleId = s.IdSample) AS TestCount
            FROM Samples s
            JOIN Patient p ON p.IdPatient = s.PatientId
            ORDER BY s.CreatedAt DESC, s.IdSample DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

            SELECT COUNT(*) FROM Samples;";

        var offset = (page - 1) * pageSize;

        using var connection = NewConnection();
        using var results = await connection.QueryMultipleAsync(sql, new { offset, pageSize });

        var items = await results.ReadAsync<SampleListItem>();
        var total = await results.ReadSingleAsync<int>();
        return (items.ToList(), total);
    }

    // ---------- Cambiar estado ----------

    public async Task<bool> UpdateStatusAsync(int id, string currentStatus, string newStatus, string? rejectionReason)
    {
        // Solo actualiza si el estado sigue siendo el que se validó antes.
        // Si otra solicitud lo cambió en el medio, no toca ninguna fila y devuelve false.
        const string sql = @"
            UPDATE Samples
            SET Status = @newStatus, RejectionReason = @rejectionReason
            WHERE IdSample = @id AND Status = @currentStatus";

        using var connection = NewConnection();
        var rowsAffected = await connection.ExecuteAsync(sql, new { id, currentStatus, newStatus, rejectionReason });
        return rowsAffected == 1;
    }
}
