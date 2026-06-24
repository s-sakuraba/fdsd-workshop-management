using System;

namespace Fdsd.Domain.Entities;

public class T_Kenshu_Document
{
    public int ID { get; set; }
    public short KENSHUCD { get; set; }
    public string DOCUMENTNAME { get; set; } = "";
    public string DOCUMENTDIR { get; set; } = "";
    public DateTime? UPDATEBI { get; set; }
    public short? UPDATEUSERID { get; set; }

    public T_Kenshu Kenshu { get; set; } = null!;
}