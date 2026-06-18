using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;
using Fdsd.Domain.Entities;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class T_LEAVE_OF_ABSENCEController : Controller
{
    private readonly LeaveOfAbsenceService _service;
    private readonly UserService _userService;

    public T_LEAVE_OF_ABSENCEController(LeaveOfAbsenceService service, UserService userService)
    {
        _service = service;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        return View(await _service.GetAllAsync(ct));
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        ViewBag.UserList = await _userService.GetActiveUsersAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(short userId, DateTime startDate, DateTime? endDate, CancellationToken ct = default)
    {
        await _service.AddAsync(new T_Leave_Of_Absence { USERID = userId, StartDate = startDate, EndDate = endDate }, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();
        ViewBag.UserList = await _userService.GetActiveUsersAsync(ct);
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, short userId, DateTime startDate, DateTime? endDate, CancellationToken ct = default)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();
        entity.USERID = userId;
        entity.StartDate = startDate;
        entity.EndDate = endDate;
        await _service.UpdateAsync(entity, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var list = await _service.GetAllAsync(ct);
        var dto = list.Find(x => x.Id == id);
        if (dto == null) return NotFound();
        return View(dto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity != null)
            await _service.RemoveAsync(entity, ct);
        return RedirectToAction(nameof(Index));
    }
}