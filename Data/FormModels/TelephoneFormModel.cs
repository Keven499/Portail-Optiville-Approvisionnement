using System.ComponentModel.DataAnnotations;

namespace Portail_OptiVille.Data.FormModels {
    public class TelephoneFormModel
    {
        public int IdTelephone { get; set; }

        [Required(ErrorMessage = "Type requis")]
        public string TypeTelEntreprise { get; set; }
    
        [Required(ErrorMessage = "No requis")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Chiffres uniquement")]
        public string NoTelEntreprise { get; set; }

        [RegularExpression(@"^\d{1,6}$", ErrorMessage = "Chiffres uniquement")]
        public string? PosteTelEntreprise { get; set; }
    }
}