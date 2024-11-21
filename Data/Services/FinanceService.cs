using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;

namespace Portail_OptiVille.Data.Services
{
    public class FinanceService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;

        public FinanceService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        public async Task UpdateFinanceData(FinanceFormModel financeFormModel, int IdFournisseur, string email)
        {
            bool isEqual = true;
            var finance = await _context.Finances.FindAsync(financeFormModel.IdFinance);
            string[] oldData = {finance.NumeroTps, finance.NumeroTvq, finance.Devise, finance.ConditionPaiement, finance.ModeCommunication};
            string[] newData = {financeFormModel.NumeroTps, financeFormModel.NumeroTvq, financeFormModel.Devise, financeFormModel.ConditionPaiement, financeFormModel.ModeCommunication};
            string[] keyData = {"TPS", "TVQ", "Devise", "Conditions de paiement", "Mode de communication"};
            var oldDict = new Dictionary<string, object> { { "Section", "Finance" } };
            var newDict = new Dictionary<string, object> { { "Section", "Finance" } };
            if (finance != null) {
                for (int i = 0; i < oldData.Length; i++)
                {
                    if (!oldData[i].Equals(newData[i]))
                    {
                        isEqual = false;
                        oldDict.Add(keyData[i], oldData[i]);
                        newDict.Add(keyData[i], newData[i]);
                    }
                }
                string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
                string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
                if (!isEqual)
                    await _historiqueService.ModifyEtat("Modifiée", IdFournisseur, email, null, oldJSON, newJSON);
                
                finance.NumeroTps = financeFormModel.NumeroTps;
                finance.NumeroTvq = financeFormModel.NumeroTvq;
                finance.Devise = financeFormModel.Devise;
                finance.ConditionPaiement = financeFormModel.ConditionPaiement;
                finance.ModeCommunication = financeFormModel.ModeCommunication;
                finance.Fournisseur = financeFormModel.IdFournisseur;
    
                _context.Finances.Update(finance);
                await _context.SaveChangesAsync();
            }
            else
            {
                var lastFinanceId = await _context.Finances.MaxAsync(f => (int?)f.IdFinance);
                try
                {
                    var financeNew = new Finance
                    {
                        IdFinance = financeFormModel.IdFinance,
                        NumeroTps = financeFormModel.NumeroTps,
                        NumeroTvq = financeFormModel.NumeroTvq,
                        Devise = financeFormModel.Devise,
                        ConditionPaiement = financeFormModel.ConditionPaiement,
                        ModeCommunication = financeFormModel.ModeCommunication,
                        Fournisseur = IdFournisseur
                    };
                    for (int i = 0; i < oldData.Length; i++)
                    {
                        if (!oldData[i].Equals(newData[i]))
                        {
                            isEqual = false;
                            newDict.Add(keyData[i], newData[i]);
                        }
                    }
                    string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
                    string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
                    if (!isEqual)
                        await _historiqueService.ModifyEtat("Modifiée", IdFournisseur, email, null, oldJSON, newJSON);

                    _context.Finances.Add(financeNew);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Une erreur est survenue lors de la sauvegarde de l'identification", ex);
                } 
            }
        }
    }
}