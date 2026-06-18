using System;
using System.Collections.Generic;

namespace Fdsd.Domain.Entities;

public class T_Kenshu
{
    public int KENSHUCD { get; set; }
    public string KENSHUNAME { get; set; } = "";
    public DateTime KENSHUDATE { get; set; }
    public DateTime? ENDDATE { get; set; }
    public short FDSDCD { get; set; }
    public short SHUSAICD { get; set; }
    public string SHUSAINAME { get; set; } = "";
    public short KSTYLECD { get; set; }
    public string? KenshuPlace { get; set; }
    public string? INFODOCU { get; set; }
    public DateTime? INSERTBI { get; set; }
    public int? INSERTUSERID { get; set; }
    public DateTime? UPDATEBI { get; set; }
    public int? UPDATEUSERID { get; set; }

    public T_Kenshu_Style KenshuStyle { get; set; } = null!;
    public ICollection<T_Kenshu_Attend> KenshuAttends { get; set; } = new List<T_Kenshu_Attend>();
    public ICollection<T_Kenshu_Gakka> KenshuGakkas { get; set; } = new List<T_Kenshu_Gakka>();
    public ICollection<T_Kenshu_Document> KenshuDocuments { get; set; } = new List<T_Kenshu_Document>();
}