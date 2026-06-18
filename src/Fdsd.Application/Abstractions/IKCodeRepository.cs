using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IKCodeRepository
{
    IQueryable<K_Code> Query();
    Task<List<K_Code>> GetByCodeNoAsync(short codeNo, CancellationToken ct = default);
}