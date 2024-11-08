using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Finance
{
    public int IdFinance { get; set; }

    public string? NumeroTps { get; set; }

    public string? NumeroTvq { get; set; }

    public string? ConditionPaiement { get; set; }

    public string Devise { get; set; } = null!;

    public string ModeCommunication { get; set; } = null!;

    public int? Fournisseur { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }
}
