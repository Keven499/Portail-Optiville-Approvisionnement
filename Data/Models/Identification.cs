using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Identification
{
    public int IdIdentification { get; set; }

    public string? Neq { get; set; }

    public string? NomEntreprise { get; set; }

    public string? AdresseCourriel { get; set; }

    public string? MotDePasse { get; set; }

    public int? Fournisseur { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }
}
