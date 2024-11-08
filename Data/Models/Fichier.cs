using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Fichier
{
    public int IdFichier { get; set; }

    public string? Nom { get; set; }

    public string? Type { get; set; }

    public int? Taille { get; set; }

    public DateTime? DateCreation { get; set; }

    public int? Fournisseur { get; set; }

    public string? Path { get; set; }

    public virtual Fournisseur? FournisseurNavigation { get; set; }
}
