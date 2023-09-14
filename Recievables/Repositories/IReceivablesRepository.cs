using System;
using Recievables.Models;

namespace Recievables.Repositories
{
    public interface IReceivablesRepository
    {
        Task AddReceivablesAsync(List<Receivable> receivables);
        Task<ReceivablesSummary> GetSummaryAsync();
    }
}

