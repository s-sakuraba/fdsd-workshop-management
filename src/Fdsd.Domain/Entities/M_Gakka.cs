using System.Collections.Generic;

namespace Fdsd.Domain.Entities;

public class M_Gakka
{
    public short GAKKACD { get; set; }
    public string GAKKANAME { get; set; } = "";
    public string GAKKARYAKU { get; set; } = "";
    public short? FDSDCD { get; set; }

    public ICollection<M_User> Users { get; set; } = new List<M_User>();
    public ICollection<T_Gakka_Change> GakkaChanges { get; set; } = new List<T_Gakka_Change>();
    public ICollection<T_Kenshu_Gakka> KenshuGakkas { get; set; } = new List<T_Kenshu_Gakka>();
}