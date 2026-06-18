using System;
using System.ComponentModel.DataAnnotations;

namespace Fdsd.Web.Models;

public class SearchConditionModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "研修会区分が未指定です")]
    public short?[]? FdsdCds { get; set; }

    [Required(ErrorMessage = "学科が未指定です")]
    public short[]? GakkaCds { get; set; }

    public string? ActName { get; set; }
    public string? MenuBtn { get; set; }
}