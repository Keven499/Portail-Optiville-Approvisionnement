using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Historique
{
    public int IdHistorique { get; set; }

    public string EtatDemande { get; set; } = null!;

    public string? RaisonRefus { get; set; }

    public DateTime? DateEtatChanged { get; set; }

    public int? Fournisseur { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }
}
