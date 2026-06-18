using System.Collections.Generic;
using Fdsd.Domain.Enums;

namespace Fdsd.Domain.Entities;

public class M_User
{
    public short USERID { get; set; }
    public short CorpCd { get; set; }
    public short EmpCd { get; set; }
    public string EmpName { get; set; } = "";
    public short GAKKACD { get; set; }
    public DateTime? NyusyaDate { get; set; }
    public DateTime? TaisyaDate { get; set; }
    public string EmpUserNm { get; set; } = "";
    public short FDSDCD { get; set; }
    public short? ZaisyokuKbn { get; set; } = 1;
    public byte? EmpKubun { get; set; }
    public byte User_Role { get; set; }

    public M_Gakka Gakka { get; set; } = null!;
    public T_Emp_Kubun EmpKubunRef { get; set; } = null!;
    public ICollection<T_Gakka_Change> GakkaChanges { get; set; } = new List<T_Gakka_Change>();
    public ICollection<T_Kenshu_Attend> KenshuAttends { get; set; } = new List<T_Kenshu_Attend>();
    public ICollection<T_Leave_Of_Absence> LeaveOfAbsences { get; set; } = new List<T_Leave_Of_Absence>();
}