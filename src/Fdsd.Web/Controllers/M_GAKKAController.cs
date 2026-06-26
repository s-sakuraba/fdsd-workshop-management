using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;
using Fdsd.Application.Common;
using Fdsd.Domain.Entities;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class M_GAKKAController : Controller
{
    private readonly GakkaService _gakkaService;
    public M_GAKKAController(GakkaService gakkaService) { _gakkaService = gakkaService; }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var list = await _gakkaService.GetAllAsync(ct);
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        ViewBag.NextGakkaCd = await _gakkaService.GetNextGakkaCdAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(short gakkaCd, string gakkaName, string? gakkaRyaku, short? fdsdCd, CancellationToken ct = default)
    {
        await _gakkaService.CreateAsync(gakkaCd, gakkaName, gakkaRyaku, fdsdCd, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(short id, CancellationToken ct = default)
    {
        var gakka = await _gakkaService.GetByIdAsync(id, ct);
        if (gakka == null) return NotFound();
        return View(gakka);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(short id, CancellationToken ct = default)
    {
        var gakka = await _gakkaService.GetByIdAsync(id, ct);
        if (gakka == null) return NotFound();
        return View(gakka);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(short id, string gakkaName, string? gakkaRyaku, short? fdsdCd, CancellationToken ct = default)
    {
        await _gakkaService.UpdateAsync(id, gakkaName, gakkaRyaku, fdsdCd, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(short id, CancellationToken ct = default)
    {
        var gakka = await _gakkaService.GetByIdAsync(id, ct);
        if (gakka == null) return NotFound();
        return View(gakka);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(short id, CancellationToken ct = default)
    {
        await _gakkaService.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}