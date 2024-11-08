using System.ComponentModel.DataAnnotations;
using Portail_OptiVille.Data.Attributes;

namespace Portail_OptiVille.Data.FormModels {
    public class ContactFormModel
    {
        public int IdContact { get; set; }

        [NomPrenom("Prénom")]
        public string Prenom { get; set; }

        [NomPrenom("Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Fonction requise")]
        [RegularExpression("^[^0-9]*$", ErrorMessage = "Ne peut contenir des chiffres")]
        public string Fonction { get; set; }

        [Required(ErrorMessage = "Courriel requis")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Format invalide")]
        public string AdresseCourriel { get; set; }

        [Required(ErrorMessage = "Type requis")]
        public string TypeTelephone { get; set; } = null!;

        [Required(ErrorMessage = "No requis")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Chiffres uniquement")]
        public string Telephone { get; set; }

        [RegularExpression(@"^\d{1,6}$", ErrorMessage = "Chiffres uniquement")]
        public string? Poste { get; set; }
    }
}