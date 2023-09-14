using Microsoft.AspNetCore.Mvc;
using Recievables.Models;
using Recievables.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Recievables.Controllers;

[ApiController]
[Route("api/receivables")]
public class ReceivablesController : ControllerBase
{
    private readonly IReceivablesRepository _repository;

    public ReceivablesController(IReceivablesRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> AddReceivables([FromBody] ReceivablePayload payload)
    {
        if (payload == null || payload.Receivables == null || !payload.Receivables.Any())
        {
            return BadRequest("Payload is empty or invalid.");
        }

        await _repository.AddReceivablesAsync(payload.Receivables);
        return Ok(payload.Receivables);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _repository.GetSummaryAsync();
        return Ok(summary);
    }
}
