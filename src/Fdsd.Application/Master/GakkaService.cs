using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fdsd.Application.Master;

public class GakkaService
{
    private readonly IGakkaRepository _gakkaRepo;
    private readonly IGakkaOrderRepository _gakkaOrderRepo;
    private readonly IUnitOfWork _uow;

    public GakkaService(IGakkaRepository gakkaRepo, IGakkaOrderRepository gakkaOrderRepo, IUnitOfWork uow)
    {
        _gakkaRepo = gakkaRepo;
        _gakkaOrderRepo = gakkaOrderRepo;
        _uow = uow;
    }

    public async Task<List<GakkaDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _gakkaRepo.GetAllOrderedAsync(ct);
        var orders = await _gakkaOrderRepo.GetAllAsync(ct);
        var orderLookup = orders.ToDictionary(o => (int)o.GAKKACD, o => (short?)o.OrderNo);

        return list.Select(g => new GakkaDto(
            g.GAKKACD, g.GAKKANAME, g.FDSDCD,
            orderLookup.GetValueOrDefault(g.GAKKACD) ?? 999
        )).ToList();
    }

    public Task<M_Gakka?> GetByIdAsync(short id, CancellationToken ct = default)
        => _gakkaRepo.GetByIdAsync(id, ct);

    public async Task<short> GetNextGakkaCdAsync(CancellationToken ct = default)
    {
        var max = await _gakkaRepo.Query().MaxAsync(x => (short?)x.GAKKACD, ct) ?? 0;
        return (short)(max + 1);
    }

    public async Task<short> GetNextFdsdCdAsync(CancellationToken ct = default)
    {
        var max = await _gakkaRepo.Query().MaxAsync(x => x.FDSDCD, ct) ?? 0;
        return (short)(max + 1);
    }

    public async Task CreateAsync(short gakkaCd, string gakkaName, string? gakkaRyaku, short? fdsdCd, CancellationToken ct = default)
    {
        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            _gakkaRepo.Add(new M_Gakka { GAKKACD = gakkaCd, GAKKANAME = gakkaName, GAKKARYAKU = gakkaRyaku ?? "", FDSDCD = fdsdCd });
            _gakkaOrderRepo.Add(new T_Gakka_Order { GAKKACD = gakkaCd, OrderNo = 0 });
            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch { await tx.RollbackAsync(ct); throw; }
    }

    public async Task UpdateAsync(short gakkaCd, string gakkaName, string? gakkaRyaku, short? fdsdCd, CancellationToken ct = default)
    {
        var gakka = await _gakkaRepo.GetByIdAsync(gakkaCd, ct);
        if (gakka == null) throw new NotFoundException(nameof(M_Gakka), gakkaCd);
        gakka.GAKKANAME = gakkaName;
        gakka.GAKKARYAKU = gakkaRyaku ?? "";
        gakka.FDSDCD = fdsdCd;
        _gakkaRepo.Update(gakka);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(short gakkaCd, CancellationToken ct = default)
    {
        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var gakka = await _gakkaRepo.GetByIdAsync(gakkaCd, ct);
            if (gakka != null) _gakkaRepo.Remove(gakka);
            var order = await _gakkaOrderRepo.Query()
                .FirstOrDefaultAsync(o => o.GAKKACD == gakkaCd, ct);
            if (order != null) _gakkaOrderRepo.Remove(order);
            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch { await tx.RollbackAsync(ct); throw; }
    }
}