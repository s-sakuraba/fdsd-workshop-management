using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Master;

public class StyleService
{
    private readonly IKenshuStyleRepository _repo;
    private readonly IM_IdManageRepository _idRepo;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public StyleService(IKenshuStyleRepository repo, IM_IdManageRepository idRepo, IUnitOfWork uow, IClock clock)
    {
        _repo = repo;
        _idRepo = idRepo;
        _uow = uow;
        _clock = clock;
    }

    public Task<List<T_Kenshu_Style>> GetAllAsync(CancellationToken ct) => _repo.GetAllOrderedAsync(ct);

    public async Task<short> CreateAsync(string name, string bikou, CancellationToken ct = default)
    {
        var id = await _idRepo.GetNextIdAsync("T_KENSHU_STYLE", ct);
        var style = new T_Kenshu_Style
        {
            ID = (short)id,
            NAME = name,
            RYAKUSHO = name.Length > 10 ? name.Substring(0, 10) : name,
            BIKO = bikou,
            SORT = 0
        };
        _repo.Add(style);
        await _uow.SaveChangesAsync(ct);
        return (short)id;
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