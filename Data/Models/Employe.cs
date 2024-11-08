using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Employe
{
    public string Courriel { get; set; } = null!;

    public string? MotDePasse { get; set; }

    public string Role { get; set; } = null!;
}
