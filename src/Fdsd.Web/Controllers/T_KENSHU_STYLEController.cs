using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class T_KENSHU_STYLEController : Controller
{
    private readonly StyleService _service;
    public T_KENSHU_STYLEController(StyleService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        return View(await _service.GetAllAsync(ct));
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string bikou, CancellationToken ct = default)
    {
        await _service.CreateAsync(name, bikou, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(short[] ids, short[] sorts, CancellationToken ct = default)
    {
        await _service.UpdateSortAsync(ids.Zip(sorts, (id, sort) => (id, sort)).ToList(), ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(short id, CancellationToken ct = default)
    {
        var style = await _service.GetAllAsync(ct);
        var item = style.Find(s => s.ID == id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(short id, CancellationToken ct = default)
    {
        await _service.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}