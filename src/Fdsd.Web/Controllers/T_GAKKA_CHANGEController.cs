using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class T_GAKKA_CHANGEController : Controller
{
    private readonly GakkaChangeService _service;
    private readonly GakkaService _gakkaService;
    private readonly UserService _userService;

    public T_GAKKA_CHANGEController(GakkaChangeService service, GakkaService gakkaService, UserService userService)
    {
        _service = service;
        _gakkaService = gakkaService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        return View(await _service.GetAllAsync(ct));
    }

    [HttpGet]
    public async Task<IActionResult> Create(int userId, CancellationToken ct = default)
    {
        var users = await _userService.GetActiveUsersAsync(ct);
        var user = users.Find(u => u.UserId == userId);
        ViewBag.UserName = user?.EmpName ?? "";
        ViewBag.UserId = userId;
        ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int userId, short gakkaCd, System.DateTime dateOfArrival, CancellationToken ct = default)
    {
        await _service.AddAsync(userId, gakkaCd, dateOfArrival, ct);
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
        await _service.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}