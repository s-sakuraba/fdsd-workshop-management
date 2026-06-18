using System;

namespace Fdsd.Domain.Entities;

public class T_Kenshu_Attend
{
    public int ID { get; set; }
    public int KENSHUCD { get; set; }
    public short USERID { get; set; }
    public short ATTEND { get; set; }
    public DateTime? UPDATEBI { get; set; }
    public int? UPDATEUSERID { get; set; }

    public T_Kenshu Kenshu { get; set; } = null!;
    public M_User User { get; set; } = null!;
}