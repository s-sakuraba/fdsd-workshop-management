namespace Fdsd.Domain.Entities;

public class T_User_Order
{
    public short USERID { get; set; }
    public short CorpCd { get; set; }
    public short EmpCd { get; set; }
    public string EmpName { get; set; } = "";
    public short GAKKACD { get; set; }
    public string GAKKANAME { get; set; } = "";
    public short OrderNo { get; set; }
}