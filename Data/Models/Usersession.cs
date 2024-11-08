using System;
using System.Collections.Generic;

namespace Portail_OptiVille.Data.Models;

public partial class Usersession
{
    public int Id { get; set; }

    public string? OwnerEmail { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? Role { get; set; }
}
