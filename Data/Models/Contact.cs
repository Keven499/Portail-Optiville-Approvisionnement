using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Contact
{
    public int IdContact { get; set; }

    public string? Prenom { get; set; }

    public string? Nom { get; set; }

    public string? Fonction { get; set; }

    public string? AdresseCourriel { get; set; }

    public int? Fournisseur { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }

    public virtual ICollection<Telephone> Telephones { get; set; } = new List<Telephone>();
}
