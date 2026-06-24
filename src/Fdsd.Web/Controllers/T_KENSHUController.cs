using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Fdsd.Application.Kenshu;
using Fdsd.Application.Master;
using Fdsd.Application.Common;
using Fdsd.Domain.Rules;
using Fdsd.Web.Models;

namespace Fdsd.Web.Controllers;

[Authorize]
public class T_KENSHUController : Controller
{
    private readonly KenshuService _kenshuService;
    private readonly UserService _userService;
    private readonly GakkaService _gakkaService;
    private readonly StyleService _styleService;
    private readonly GakkaChangeService _gakkaChangeService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

    private string InfoDocuPath => _config["FilePaths:InfoDocuPath"] ?? @"\\seiyo-srv3\FDSD\infodocu\";
    private string DocuPath => _config["FilePaths:DocuPath"] ?? @"\\seiyo-srv3\FDSD\document\";
    private string WorkFilePath => _config["FilePaths:WorkFilePath"] ?? @"\\seiyo-srv3\FDSD\document\work\";

    public T_KENSHUController(KenshuService kenshuService, UserService userService,
        GakkaService gakkaService, StyleService styleService,
        GakkaChangeService gakkaChangeService,
        Microsoft.Extensions.Configuration.IConfiguration config)
    {
        _kenshuService = kenshuService;
        _userService = userService;
        _gakkaService = gakkaService;
        _styleService = styleService;
        _gakkaChangeService = gakkaChangeService;
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
    // S008 メニュー
    //=================================================================
    public async Task<IActionResult> Menu(CancellationToken ct = default)
    {
        var account = GetLoginAccount();
        if (!await _userService.ExistsAsync(account, ct))
            return View("NoUser");

var user = await _userService.GetByEmpUserNmAsync(account, ct);
    ViewBag.ManageBtnVisible = user?.User_Role == 1;
    return View();
    }

    [HttpPost]
    public IActionResult Menu(string? actName, string? menuBtn)
    {
        var action = menuBtn ?? actName;
        return action switch
        {
            "研修登録" => RedirectToAction(nameof(Create)),
            "研修編集" => RedirectToAction(nameof(SearchCondition), new { actName = "研修編集" }),
            "出欠入力" => RedirectToAction(nameof(SearchCondition), new { actName = "出欠入力" }),
            "帳票出力" => RedirectToAction(nameof(SearchCondition), new { actName = "帳票出力" }),
            "学科登録・編集" => RedirectToAction("Index", "M_GAKKA"),
            "教職員登録・編集" => RedirectToAction("Index", "M_USER"),
            "従業員区分登録・編集" => RedirectToAction("Index", "T_EMP_KUBUN"),
            "所属異動管理" => RedirectToAction("Index", "T_GAKKA_CHANGE"),
            "休職管理" => RedirectToAction("Index", "T_LEAVE_OF_ABSENCE"),
            "学科並び順編集" => RedirectToAction("Index", "T_GAKKA_ORDER"),
            "教職員並び順編集" => RedirectToAction("Index", "T_USER_ORDER"),
            "開催形態編集" => RedirectToAction("Index", "T_KENSHU_STYLE"),
            _ => View("NoUser")
        };
    }

    //=================================================================
    // S001 研修検索・出力画面
    //=================================================================
    public async Task<IActionResult> SearchCondition(string? actName, CancellationToken ct = default)
    {
        ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
        ViewBag.ActName = actName ?? "研修編集";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Search(SearchConditionModel model)
    {
        if (model.FdsdCds == null || model.FdsdCds.Length == 0)
        {
            ViewBag.ErrMsg = "【研修会区分】が未指定です";
            return View("../ShowWarning/DataNullOrEmpty");
        }
        if (model.GakkaCds == null || model.GakkaCds.Length == 0)
        {
            ViewBag.ErrMsg = "【学科】が未指定です";
            return View("../ShowWarning/DataNullOrEmpty");
        }

        TempData["FDSDCondition"] = System.Text.Json.JsonSerializer.Serialize(model.FdsdCds);
        TempData["GAKKACondition"] = System.Text.Json.JsonSerializer.Serialize(model.GakkaCds);
        TempData["StartDate"] = model.StartDate?.ToString("yyyyMMdd");
        TempData["EndDate"] = model.EndDate?.ToString("yyyyMMdd");
        TempData["MenuBtn"] = model.MenuBtn ?? model.ActName ?? "研修編集";

        switch (model.ActName)
        {
            case "検索":
            case "戻る":
            case "処理完了":
                return RedirectToAction(nameof(GetSearchResult));
            case "学科別 受講者出力":
                return RedirectToAction("Report_P001", "Report");
            case "未受講者出力":
                return RedirectToAction("Report_P002", "Report");
            case "研修実績一覧表出力":
                return RedirectToAction("Report_P003", "Report");
            default:
                return BadRequest();
        }
    }

    //=================================================================
    // S002 検索結果画面
    //=================================================================
    public async Task<IActionResult> GetSearchResult(CancellationToken ct = default)
    {
        SearchConditionDto condition = new(null, null, null, []);
        if (TempData["FDSDCondition"] is string fdsdJson)
            condition = condition with { FdsdCds = System.Text.Json.JsonSerializer.Deserialize<short?[]>(fdsdJson) };
        if (TempData["GAKKACondition"] is string gakkaJson)
            condition = condition with { GakkaCds = System.Text.Json.JsonSerializer.Deserialize<short[]>(gakkaJson) ?? [] };
        if (TempData["StartDate"] is string sDate && DateTime.TryParseExact(sDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var sd))
            condition = condition with { StartDate = sd };
        if (TempData["EndDate"] is string eDate && DateTime.TryParseExact(eDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var ed))
            condition = condition with { EndDate = ed };

        var results = await _kenshuService.SearchAsync(condition, ct);
        TempData.Keep();
        ViewBag.MenuBtn = TempData["MenuBtn"] as string ?? "研修編集";
        return View("Search", results);
    }

    //=================================================================
    // S003 研修登録画面
    //=================================================================
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
        ViewBag.StyleList = await _styleService.GetAllAsync(ct);
        return View(new KenshuCreateModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KenshuCreateModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
            ViewBag.StyleList = await _styleService.GetAllAsync(ct);
            return View(model);
        }

        var loginUserId = await GetLoginUserIdAsync(ct);

        string? infoDocu = null;
        if (model.UploadFile != null)
        {
            infoDocu = model.UploadFile.FileName;
        }

        var kenshuCd = await _kenshuService.CreateAsync(
            model.KenshuName, model.KenshuDate, model.EndDate,
            model.FdsdCd, model.ShusaiCd, model.ShusaiName,
            model.StyleCd, model.KenshuPlace,
            model.GakkaCds ?? [], infoDocu, loginUserId, ct);

        if (model.UploadFile != null)
        {
            var dir = System.IO.Path.Combine(InfoDocuPath, kenshuCd.ToString());
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, model.UploadFile.FileName);
            using var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            await model.UploadFile.CopyToAsync(stream, ct);
        }

        return RedirectToAction(nameof(SearchCondition));
    }

    //=================================================================
    // S004 研修詳細
    //=================================================================
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct = default)
    {
        var detail = await _kenshuService.GetDetailAsync(id, ct);
        if (detail == null) return NotFound();
        return View(detail);
    }

    [HttpGet]
    public IActionResult InfoDocuDownload(int id, string? targetfile)
    {
        if (string.IsNullOrEmpty(targetfile)) return NotFound();
        var path = System.IO.Path.Combine(InfoDocuPath, id.ToString(), targetfile);
        if (!System.IO.File.Exists(path)) return NotFound();
        var bytes = System.IO.File.ReadAllBytes(path);
        return File(bytes, "application/octet-stream", targetfile);
    }

    [HttpGet]
    public IActionResult DocumentDownload(int id, string? targetfile)
    {
        if (string.IsNullOrEmpty(targetfile)) return NotFound();
        var path = System.IO.Path.Combine(DocuPath, id.ToString(), targetfile);
        if (!System.IO.File.Exists(path)) return NotFound();
        var bytes = System.IO.File.ReadAllBytes(path);
        return File(bytes, "application/octet-stream", targetfile);
    }

    //=================================================================
    // S006 研修削除
    //=================================================================
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var detail = await _kenshuService.GetDetailAsync(id, ct);
        if (detail == null) return NotFound();
        return View(detail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
    {
        await _kenshuService.DeleteAsync(id, ct);
        return RedirectToAction(nameof(GetSearchResult));
    }

    //=================================================================
    // S005 研修編集
    //=================================================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
    {
        var detail = await _kenshuService.GetDetailAsync(id, ct);
        if (detail == null) return NotFound();

        ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
        ViewBag.StyleList = await _styleService.GetAllAsync(ct);
        return View(detail);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KenshuCreateModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.GakkaList = await _gakkaService.GetAllAsync(ct);
            ViewBag.StyleList = await _styleService.GetAllAsync(ct);
            return View(await _kenshuService.GetDetailAsync(id, ct));
        }

        var loginUserId = await GetLoginUserIdAsync(ct);

        string? infoDocu = model.UploadFile != null ? model.UploadFile.FileName : model.InfoDocu;
        if (model.UploadFile != null)
        {
            var dir = System.IO.Path.Combine(InfoDocuPath, id.ToString());
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            var filePath = System.IO.Path.Combine(dir, model.UploadFile.FileName);
            using var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            await model.UploadFile.CopyToAsync(stream, ct);
        }

        await _kenshuService.UpdateAsync(
            id, model.KenshuName, model.KenshuDate, model.EndDate,
            model.FdsdCd, model.ShusaiCd, model.ShusaiName,
            model.StyleCd, model.KenshuPlace,
            model.GakkaCds ?? [], infoDocu, loginUserId, ct);

        return RedirectToAction(nameof(GetSearchResult));
    }
}