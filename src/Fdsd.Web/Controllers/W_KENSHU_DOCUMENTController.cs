using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Fdsd.Application.Kenshu;
using Fdsd.Application.Master;

namespace Fdsd.Web.Controllers;

[Authorize]
public class W_KENSHU_DOCUMENTController : Controller
{
    private readonly KenshuService _kenshuService;
    private readonly UserService _userService;
    private readonly IConfiguration _config;

    private string DocuPath => _config["FilePaths:DocuPath"] ?? @"\\seiyo-srv3\FDSD\document\";

    public W_KENSHU_DOCUMENTController(KenshuService kenshuService, UserService userService, IConfiguration config)
    {
        _kenshuService = kenshuService;
        _userService = userService;
        _config = config;
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

    //=================================================================
    // 資料追加
    //=================================================================
    [HttpGet]
    public IActionResult Create(int id)
    {
        ViewBag.KenshuCd = id;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int kenshuCd, IFormFile? uploadFile, CancellationToken ct = default)
    {
        if (uploadFile == null || uploadFile.Length == 0)
        {
            ViewBag.KenshuCd = kenshuCd;
            ModelState.AddModelError("", "ファイルを選択してください。");
            return View();
        }

        var dir = System.IO.Path.Combine(DocuPath, kenshuCd.ToString());
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        var fileName = System.IO.Path.GetFileName(uploadFile.FileName);
        var filePath = System.IO.Path.Combine(dir, fileName);
        using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
        {
            await uploadFile.CopyToAsync(stream, ct);
        }

        var loginUserId = await GetLoginUserIdAsync(ct);
        await _kenshuService.AddDocumentAsync(kenshuCd, fileName, dir, loginUserId, ct);

        return RedirectToAction("Edit", "T_KENSHU", new { id = kenshuCd });
    }

    //=================================================================
    // 資料削除
    //=================================================================
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var doc = await _kenshuService.GetDocumentAsync(id, ct);
        if (doc == null) return NotFound();
        return View(doc);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        var doc = await _kenshuService.GetDocumentAsync(id, ct);
        if (doc == null) return NotFound();

        var kenshuCd = doc.KENSHUCD;

        // 物理ファイルの削除
        var filePath = System.IO.Path.Combine(doc.DOCUMENTDIR, doc.DOCUMENTNAME);
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        await _kenshuService.DeleteDocumentAsync(id, ct);

        return RedirectToAction("Edit", "T_KENSHU", new { id = kenshuCd });
    }
}