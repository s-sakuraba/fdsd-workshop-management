using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Master;

public class StyleService
{
    private readonly IKenshuStyleRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public StyleService(IKenshuStyleRepository repo, IUnitOfWork uow, IClock clock)
    {
        _repo = repo;
        _uow = uow;
        _clock = clock;
    }

    public Task<List<T_Kenshu_Style>> GetAllAsync(CancellationToken ct) => _repo.GetAllOrderedAsync(ct);

    public async Task<short> GetNextSortAsync(CancellationToken ct = default)
    {
        var max = await _repo.Query().MaxAsync(x => (short?)x.SORT, ct) ?? 0;
        return (short)(max + 1);
    }

    public async Task<short> CreateAsync(string name, short sort, string? bikou, CancellationToken ct = default)
    {
        var style = new T_Kenshu_Style
        {
            NAME = name,
            RYAKUSHO = name.Length > 10 ? name.Substring(0, 10) : name,
            BIKO = bikou,
            SORT = sort
        };
        _repo.Add(style);
        await _uow.SaveChangesAsync(ct);
        return style.ID;
    }

    public async Task UpdateSortAsync(List<(short id, short sort)> items, CancellationToken ct = default)
    {
        foreach (var (id, sort) in items)
        {
            var style = await _repo.GetByIdAsync(id, ct);
            if (style == null) continue;
            style.SORT = sort;
            _repo.Update(style);
        }
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(short id, CancellationToken ct = default)
    {
        var style = await _repo.GetByIdAsync(id, ct);
        if (style == null) throw new NotFoundException(nameof(T_Kenshu_Style), id);
        _repo.Remove(style);
        await _uow.SaveChangesAsync(ct);
    }
}