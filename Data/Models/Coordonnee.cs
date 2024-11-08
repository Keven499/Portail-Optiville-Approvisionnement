using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Coordonnee
{
    public int IdCoordonnee { get; set; }

    public string? NoCivique { get; set; }

    public string? Rue { get; set; }

    public string? Bureau { get; set; }

    public string? Ville { get; set; }

    public string? Province { get; set; }

    public string? CodePostal { get; set; }

    public string? CodeRegionAdministrative { get; set; }

    public string? RegionAdministrative { get; set; }

    public string? SiteInternet { get; set; }

    public int? Fournisseur { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }

    public virtual ICollection<Telephone> Telephones { get; set; } = new List<Telephone>();
}
