using System;

namespace Fdsd.Domain.Entities;

public class T_Kenshu_Gakka
{
    public short KENSHUCD { get; set; }
    public short GAKKACD { get; set; }
    public DateTime? UPDATEBI { get; set; }
    public short? UPDATEUSERID { get; set; }

    public T_Kenshu Kenshu { get; set; } = null!;
    public M_Gakka Gakka { get; set; } = null!;
}