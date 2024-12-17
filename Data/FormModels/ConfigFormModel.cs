using System.ComponentModel.DataAnnotations;

namespace Portail_OptiVille.Data.FormModels {
    public class ConfigFormModel {

        [Required(ErrorMessage = "Courriel d'approvisionnement requis")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Format invalide")]
        public string CourrielAppro { get; set; }

        [Required(ErrorMessage = "Courriel des finances requis")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Format invalide")]
        public string CourrielFinance { get; set; }

        [Required(ErrorMessage = "Délai avant révision requis")]
        [Range(1, 48, ErrorMessage = "Entre 1 et 48 mois")]
        public int DelaiBeforeRevision { get; set; }

        [Required(ErrorMessage = "Taille des fichiers requis")]
        [Range(1, 1000, ErrorMessage = "Entre 1 et 1000 MO")]
        public int MaxFileSize { get; set; }

        [Required(ErrorMessage = "Limite de fichiers requise")]
        public int MaxFileLimite { get; set; }
    }
}