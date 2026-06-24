using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Attend;
using Fdsd.Application.Common;
using Fdsd.Application.Master;

namespace Fdsd.Web.Controllers;

[Authorize]
public class T_KENSHU_ATTENDController : Controller
{
    private readonly AttendService _attendService;
    private readonly UserService _userService;

    public T_KENSHU_ATTENDController(AttendService attendService, UserService userService)
    {
        _attendService = attendService;
        _userService = userService;
    }

    private string GetLoginAccount()
    {
        var identity = User.Identity?.Name ?? "";
        var idx = identity.IndexOf('\\');
        return idx >= 0 ? identity.Substring(idx + 1) : identity;
    }

    private async Task<int> GetLoginUserIdAsync(CancellationToken ct = default)
    {
        var account = GetLoginAccount();
        var user = await _userService.GetByEmpUserNmAsync(account, ct);
        return user?.USERID ?? 0;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var list = await _attendService.GetAttendListAsync(id, ct);
        ViewBag.KenshuName = await _attendService.GetKenshuNameAsync(id, ct);
        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, List<AttendItemDto>? attendList, CancellationToken ct = default)
    {
        var loginUserId = await GetLoginUserIdAsync(ct);

        var data = (attendList ?? new List<AttendItemDto>())
            .Select(x => (x.UserId, x.Attend)).ToList();
        await _attendService.SaveAttendsAsync(id, data, loginUserId, ct);
        return RedirectToAction("GetSearchResult", "T_KENSHU");
    }
}