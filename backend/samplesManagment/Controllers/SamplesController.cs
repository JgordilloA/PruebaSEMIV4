using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using samplesManagment.DTOs;
using samplesManagment.Services;

namespace samplesManagment.Controllers;

[ApiController]
[Route("api/samples")]
public class SamplesController(ISampleService service) : ControllerBase
{
    // POST /api/samples
    [HttpPost]
    public async Task<IActionResult> Create(CreateSampleRequest request)
    {
        var sample = await service.CreateAsync(request);
        return StatusCode(StatusCodes.Status201Created, sample);
    }

    // GET /api/samples?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor o igual a 1.")] int page = 1,
        [Range(1, 50, ErrorMessage = "El tamaño de página debe estar entre 1 y 50.")] int pageSize = 10)
    {
        return Ok(await service.GetPageAsync(page, pageSize));
    }

    // PATCH /api/samples/{id}/status
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeStatusRequest request)
    {
        return Ok(await service.ChangeStatusAsync(id, request));
    }
}
