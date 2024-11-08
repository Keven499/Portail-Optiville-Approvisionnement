using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Licencerbq
{
    public string IdLicenceRbq { get; set; } = null!;

    public string Statut { get; set; } = null!;

    public string? Type { get; set; }

    public int? Fournisseur { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }

    public virtual ICollection<Categorierbq> IdCategorieRbqs { get; set; } = new List<Categorierbq>();
}
