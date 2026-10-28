using samplesManagment.DTOs;

namespace samplesManagment.Services;

public interface ISampleService
{
    Task<SampleDetail> CreateAsync(CreateSampleRequest request);
    Task<PagedResult<SampleListItem>> GetPageAsync(int page, int pageSize);
    Task<SampleDetail> ChangeStatusAsync(int id, ChangeStatusRequest request);
}
