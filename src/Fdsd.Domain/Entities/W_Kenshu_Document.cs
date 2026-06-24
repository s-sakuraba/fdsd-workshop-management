using System;

namespace Fdsd.Domain.Entities;

public class W_Kenshu_Document
{
    public int ID { get; set; }
    public short KENSHUCD { get; set; }
    public string DOCUMENTNAME { get; set; } = "";
    public string DOCUMENTDIR { get; set; } = "";
    public DateTime? UPDATEBI { get; set; }
    public short UPDATEUSERID { get; set; }
    public short DELFLG { get; set; }
}