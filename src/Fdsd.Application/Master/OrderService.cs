using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;

namespace Fdsd.Application.Master;

public class OrderService
{
    private readonly IGakkaOrderRepository _gakkaOrderRepo;
    private readonly IUserOrderRepository _userOrderRepo;
    private readonly IUnitOfWork _uow;

    public OrderService(IGakkaOrderRepository gakkaOrderRepo, IUserOrderRepository userOrderRepo, IUnitOfWork uow)
    {
        _gakkaOrderRepo = gakkaOrderRepo;
        _userOrderRepo = userOrderRepo;
        _uow = uow;
    }

    public Task<List<T_Gakka_Order>> GetAllGakkaOrdersAsync(CancellationToken ct) => _gakkaOrderRepo.GetAllAsync(ct);
    public Task<List<T_User_Order>> GetAllUserOrdersAsync(CancellationToken ct) => _userOrderRepo.GetAllAsync(ct);

    public async Task SaveGakkaOrdersAsync(List<T_Gakka_Order> orders, CancellationToken ct = default)
    {
        var existing = await _gakkaOrderRepo.GetAllAsync(ct);
        var existingCd = existing.Select(x => x.GAKKACD).ToHashSet();
        foreach (var o in orders)
        {
            if (existingCd.Contains(o.GAKKACD))
                _gakkaOrderRepo.Update(o);
            else
                _gakkaOrderRepo.Add(o);
        }
        await _uow.SaveChangesAsync(ct);
    }

    public async Task SaveUserOrdersAsync(List<T_User_Order> orders, CancellationToken ct = default)
    {
        var existing = await _userOrderRepo.GetAllAsync(ct);
        var existingIds = existing.Select(x => x.USERID).ToHashSet();
        foreach (var o in orders)
        {
            if (existingIds.Contains(o.USERID))
                _userOrderRepo.Update(o);
            else
                _userOrderRepo.Add(o);
        }
        await _uow.SaveChangesAsync(ct);
    }
}