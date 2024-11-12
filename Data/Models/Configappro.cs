using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Configappro
{
    public int IdConfigAppro { get; set; }

    public string CourrielAppro { get; set; } = null!;

    public int DelaiRevision { get; set; }

    public int TailleMaxFichiers { get; set; }

    public string CourrielFinance { get; set; } = null!;
}
