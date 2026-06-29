using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;
using Fdsd.Domain.Entities;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class T_USER_ORDERController : Controller
{
    private readonly OrderService _service;
    public T_USER_ORDERController(OrderService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var orders = await _service.GetAllUserOrdersAsync(ct);

        // 負の値（-1以下）を 0 に補正する（0は許容）
        foreach (var o in orders)
        {
            if (o.OrderNo < 0) o.OrderNo = 0;
        }

        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(List<T_User_Order> orders, CancellationToken ct = default)
    {
        await _service.SaveUserOrdersAsync(orders, ct);
        return RedirectToAction(nameof(Index));
    }
}