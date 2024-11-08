using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Telephone
{
    public int IdTelephone { get; set; }

    public string Type { get; set; } = null!;

    public string? NumTelephone { get; set; }

    public string? Poste { get; set; }

    public int? Contact { get; set; }

    public int? Coordonnee { get; set; }

    public virtual Contact? ContactNavigation { get; set; }

    public virtual Coordonnee? CoordonneeNavigation { get; set; }
}
