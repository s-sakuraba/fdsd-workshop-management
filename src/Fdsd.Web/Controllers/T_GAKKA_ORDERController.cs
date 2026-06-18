using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;
using Fdsd.Domain.Entities;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class T_GAKKA_ORDERController : Controller
{
    private readonly OrderService _service;
    private readonly GakkaService _gakkaService;

    public T_GAKKA_ORDERController(OrderService service, GakkaService gakkaService)
    {
        _service = service;
        _gakkaService = gakkaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var orders = await _service.GetAllGakkaOrdersAsync(ct);
        var gakkas = await _gakkaService.GetAllAsync(ct);
        ViewBag.GakkaNamesLookup = gakkas.ToDictionary(g => g.GakkaCd, g => g.GakkaName);
        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(List<T_Gakka_Order> orders, CancellationToken ct = default)
    {
        await _service.SaveGakkaOrdersAsync(orders, ct);
        return RedirectToAction(nameof(Index));
    }
}