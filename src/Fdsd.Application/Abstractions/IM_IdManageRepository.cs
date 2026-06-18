using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IM_IdManageRepository
{
    Task<int> GetNextIdAsync(string tableName, CancellationToken ct = default);
}