using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;
using Fdsd.Domain.Entities;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class T_EMP_KUBUNController : Controller
{
    private readonly EmpKubunService _service;
    public T_EMP_KUBUNController(EmpKubunService service) { _service = service; }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        return View(await _service.GetAllAsync(ct));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(T_Emp_Kubun entity, CancellationToken ct = default)
    {
        await _service.CreateAsync(entity, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(T_Emp_Kubun entity, CancellationToken ct = default)
    {
        await _service.UpdateAsync(entity, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(short id, CancellationToken ct = default)
    {
        await _service.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}