using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Abstractions;

public interface IKenshuStyleRepository
{
    IQueryable<T_Kenshu_Style> Query();
    Task<List<T_Kenshu_Style>> GetAllOrderedAsync(CancellationToken ct = default);
    Task<T_Kenshu_Style?> GetByIdAsync(short id, CancellationToken ct = default);
    void Add(T_Kenshu_Style style);
    void Update(T_Kenshu_Style style);
    void Remove(T_Kenshu_Style style);
}