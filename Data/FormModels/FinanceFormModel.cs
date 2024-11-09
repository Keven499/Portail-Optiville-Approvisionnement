using System.ComponentModel.DataAnnotations;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.FormModels {
    public class FinanceFormModel
    {
        public int IdFinance { get; set; }

        [Required(ErrorMessage = "TPS requis")]
        public string NumeroTps { get; set; }

        [Required(ErrorMessage = "TVQ requis")]
        public string NumeroTvq { get; set; }

        public string? Devise { get; set; }
        [Required(ErrorMessage = "Condition requise")]
        
        public string? ConditionPaiement { get; set; }
        public string? ModeCommunication { get; set; }
        public int? IdFournisseur { get; set; }

        public void FillData(Finance finance)
        {
            IdFinance = finance.IdFinance;
            NumeroTps = finance.NumeroTps;
            NumeroTvq = finance.NumeroTvq;
            Devise = finance.Devise;
            ConditionPaiement = finance.ConditionPaiement;
            ModeCommunication = finance.ModeCommunication;
            IdFournisseur = finance.Fournisseur;
        }
    }
}