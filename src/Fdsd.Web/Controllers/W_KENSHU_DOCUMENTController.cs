using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fdsd.Application.Abstractions;
using Fdsd.Domain.Entities;

namespace Fdsd.Web.Controllers;

[Authorize]
public class W_KENSHU_DOCUMENTController : Controller
{
    private readonly IWKenshuDocumentRepository _repo;
    private readonly IUnitOfWork _uow;

    public W_KENSHU_DOCUMENTController(IWKenshuDocumentRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int kenshuCd, string documentName, string documentDir, CancellationToken ct = default)
    {
        var doc = new W_Kenshu_Document
        {
            KENSHUCD = kenshuCd,
            UPDATEUSERID = 0,
            DOCUMENTNAME = documentName,
            DOCUMENTDIR = documentDir,
            DELFLG = 0
        };
        _repo.Add(doc);
        await _uow.SaveChangesAsync(ct);
        return RedirectToAction("GetSearchResult", "T_KENSHU");
    }

    public IActionResult Delete() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var docs = _repo.Query().Where(x => x.ID == id).ToList();
        if (docs.Any())
            _repo.RemoveRange(docs);
        await _uow.SaveChangesAsync(ct);
        return RedirectToAction("GetSearchResult", "T_KENSHU");
    }
}