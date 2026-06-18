using System;

namespace Fdsd.Domain.Entities;

public class T_Leave_Of_Absence
{
    public int ID { get; set; }
    public short USERID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public M_User User { get; set; } = null!;
}