using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.Services
{
    public class FinanceService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public FinanceService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
        }

        public async Task UpdateFinanceData(FinanceFormModel financeFormModel, int IdFournisseur)
        {
            var finance = await _context.Finances.FindAsync(financeFormModel.IdFinance);

             if(finance != null){
            
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