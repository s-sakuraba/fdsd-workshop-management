using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Fdsd.Web.Models;

public class KenshuCreateModel
{
    [Required(ErrorMessage = "研修会名称が空白です")]
    public string KenshuName { get; set; } = "";

    [Required(ErrorMessage = "主催者名が空白です")]
    public string ShusaiName { get; set; } = "";

    [Required]
    public DateTime KenshuDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public short FdsdCd { get; set; }

    [Required]
    public short ShusaiCd { get; set; }

    [Required]
    public short StyleCd { get; set; }

    public string? KenshuPlace { get; set; }

    [Required(ErrorMessage = "学科が未指定です")]
    public short[]? GakkaCds { get; set; }

    public IFormFile? UploadFile { get; set; }

    public string? InfoDocu { get; set; }
}