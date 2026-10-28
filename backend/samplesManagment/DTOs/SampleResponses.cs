namespace samplesManagment.DTOs;

public class TestItem
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

// Muestra con sus exámenes (respuesta de crear y cambiar estado)
public class SampleDetail
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = "";
    public string? FullName { get; set; }
    public string SampleType { get; set; } = "";
    public DateTime CollectedAt { get; set; }
    public string Status { get; set; } = "";
    public string? RejectionReason { get; set; }
    public List<TestItem> Tests { get; set; } = [];
}

// Fila del listado
public class SampleListItem
{
    public int Id { get; set; }
    public string DocumentNumber { get; set; } = "";
    public string? PatientName { get; set; }
    public string SampleType { get; set; } = "";
    public DateTime CollectedAt { get; set; }
    public string Status { get; set; } = "";
    public int TestCount { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
