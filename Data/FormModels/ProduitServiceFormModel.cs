using System.ComponentModel.DataAnnotations;
using Portail_OptiVille.Data.Attributes;

namespace Portail_OptiVille.Data.FormModels
{
    public class ProduitServiceFormModel
    {
        [Required(ErrorMessage = "Détails requis")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Limite dépassée")]
        public string? Message { get; set; }

        [NothingSelected(ErrorMessage = "Aucune case n'a été coché")]
        public Dictionary<string, bool> SousProduitSelected { get; set; } = new Dictionary<string, bool>();

        public List<string> CodeUNSPSC { get; set; } = new List<string>();
    }
}
