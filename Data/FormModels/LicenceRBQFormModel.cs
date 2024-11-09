using System.ComponentModel.DataAnnotations;
using Portail_OptiVille.Data.Attributes;

namespace Portail_OptiVille.Data.FormModels
{
    public class LicenceRBQFormModel
    {
        public string? NumeroLicence { get; set; }

        [Required(ErrorMessage = "Statut requis")]
        public string? StatutLicence { get; set; }

        [Required(ErrorMessage = "Type requis")]
        public string? TypeLicence { get; set; }

        [NothingSelected(ErrorMessage = "Aucune case n'a été coché")]
        public Dictionary<string, bool> SousCategoSelected { get; set; } = new Dictionary<string, bool>();

        public bool Disabled = false;

        public List<string> CodeSousCategorie { get; set; } = new List<string>();
    }
}
