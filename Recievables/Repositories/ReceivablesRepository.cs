using System;
using Recievables.Data;
using Recievables.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Recievables.Repositories;

public class ReceivablesSummary
{
    public decimal OpenInvoicesValue { get; set; }
    public decimal ClosedInvoicesValue { get; set; }
}

public class ReceivablesRepository : IReceivablesRepository
{
    private readonly AppDbContext _context;

    public ReceivablesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddReceivablesAsync(List<Receivable> receivables)
    {
        _context.Receivables.AddRange(receivables);
        await _context.SaveChangesAsync();
    }

    public async Task<ReceivablesSummary> GetSummaryAsync()
    {
        var openInvoicesValue = await _context.Receivables
            .Where(r => !r.Cancelled && string.IsNullOrEmpty(r.ClosedDate))
            .SumAsync(r => r.OpeningValue);

        var closedInvoicesValue = await _context.Receivables
            .Where(r => !r.Cancelled && !string.IsNullOrEmpty(r.ClosedDate))
            .SumAsync(r => r.PaidValue);

        return new ReceivablesSummary
        {
            OpenInvoicesValue = openInvoicesValue,
            ClosedInvoicesValue = closedInvoicesValue
        };
    }
}
