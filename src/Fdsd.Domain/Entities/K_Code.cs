using System.Collections.Generic;

namespace Fdsd.Domain.Entities;

public class K_Code
{
    public short CODENO { get; set; }
    public short CODE { get; set; }
    public string NAME { get; set; } = "";
    public string RYAKUSHO { get; set; } = "";
    public short SORT { get; set; }
    public string BIKO { get; set; } = "";
}