using System;

namespace Fdsd.Domain.Entities;

public class T_Gakka_Change
{
    public int ID { get; set; }
    public short USERID { get; set; }
    public short GAKKACD { get; set; }
    public DateTime DateOfArrival { get; set; }
    public DateTime? DateOfDeparture { get; set; }

    public M_User User { get; set; } = null!;
    public M_Gakka Gakka { get; set; } = null!;
}