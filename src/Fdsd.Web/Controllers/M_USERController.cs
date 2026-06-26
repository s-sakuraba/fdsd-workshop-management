using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Master;

namespace Fdsd.Web.Controllers;

[Authorize(Policy = "Admin")]
public class M_USERController : Controller
{
    private readonly UserService _userService;
    private readonly GakkaService _gakkaService;
    private readonly EmpKubunService _empKubunService;

    public M_USERController(UserService userService, GakkaService gakkaService, EmpKubunService empKubunService)
    {
        _userService = userService;
        _gakkaService = gakkaService;
        _empKubunService = empKubunService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var users = await _userService.GetActiveUsersAsync(ct);
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
        ViewBag.EmpKubunList = await _empKubunService.GetAllAsync(ct);
        ViewBag.NextEmpCd = await _userService.GetNextEmpCdAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string empName, string empUserNm, short gakkaCd,
        DateTime nyusyaDate, byte? empKubun, short empCd, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(empName) || string.IsNullOrWhiteSpace(empUserNm))
        {
            ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
            ViewBag.EmpKubunList = await _empKubunService.GetAllAsync(ct);
            ViewBag.ErrMsg = "必須項目が未入力です";
            return View();
        }
        await _userService.CreateAsync(empName, empUserNm, gakkaCd, nyusyaDate, empKubun, empCd, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var users = await _userService.GetActiveUsersAsync(ct);
        var user = users.Find(u => u.UserId == id);
        if (user == null) return NotFound();
        ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
        ViewBag.EmpKubunList = await _empKubunService.GetAllAsync(ct);
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string empName, short gakkaCd, byte? empKubun,
        short empCd, string empUserNm, CancellationToken ct = default)
    {
        await _userService.UpdateAsync(id, empName, gakkaCd, empKubun, empCd, empUserNm, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var users = await _userService.GetActiveUsersAsync(ct);
        var user = users.Find(u => u.UserId == id);
        if (user == null) return NotFound();
        ViewBag.EmpKubunName = await GetEmpKubunNameAsync(user.EmpKubun, ct);
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var users = await _userService.GetActiveUsersAsync(ct);
        var user = users.Find(u => u.UserId == id);
        if (user == null) return NotFound();
        ViewBag.EmpKubunName = await GetEmpKubunNameAsync(user.EmpKubun, ct);
        return View(user);
    }

    private async Task<string> GetEmpKubunNameAsync(byte? empKubun, CancellationToken ct)
    {
        if (empKubun == null) return "未設定";
        var list = await _empKubunService.GetAllAsync(ct);
        return list.FirstOrDefault(e => e.EmpKubun == empKubun.Value)?.KubunName ?? "未設定";
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, DateTime taisyaDate, CancellationToken ct = default)
    {
        await _userService.DeleteAsync(id, taisyaDate, ct);
        return RedirectToAction(nameof(Index));
    }
}