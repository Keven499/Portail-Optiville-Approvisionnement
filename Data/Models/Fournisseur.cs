using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Fournisseur
{
    public int IdFournisseur { get; set; }

    public string? DetailSpecification { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateLastChanged { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Coordonnee> Coordonnees { get; set; } = new List<Coordonnee>();

    public virtual ICollection<Fichier> Fichiers { get; set; } = new List<Fichier>();

    public virtual ICollection<Finance> Finances { get; set; } = new List<Finance>();

    public virtual ICollection<Historique> Historiques { get; set; } = new List<Historique>();

    public virtual ICollection<Identification> Identifications { get; set; } = new List<Identification>();

    public virtual ICollection<Licencerbq> Licencerbqs { get; set; } = new List<Licencerbq>();

    public virtual ICollection<Produitservice> IdProduitServices { get; set; } = new List<Produitservice>();
}
