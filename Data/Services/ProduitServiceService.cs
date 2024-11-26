using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;

namespace Portail_OptiVille.Data.Services
{
    public class ProduitServiceService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;

        public ProduitServiceService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        public async Task SaveProduitServiceData(ProduitServiceFormModel produitServiceFormModelDto)
        {
            var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
            var fournisseur = await _context.Fournisseurs
                .Include(f => f.IdProduitServices)
                .FirstOrDefaultAsync(f => f.IdFournisseur == lastFournisseurId);

            if (fournisseur == null)
            {
                throw new Exception("Fournisseur not found.");
            }

            try
            {
                fournisseur.DetailSpecification = produitServiceFormModelDto.Message;

                foreach (var codeUNSPSC in produitServiceFormModelDto.CodeUNSPSC)
                {
                    var produitService = await _context.Produitservices 
                        .FirstOrDefaultAsync(p => p.CodeUnspsc == codeUNSPSC);

                    if (produitService != null && !fournisseur.IdProduitServices.Contains(produitService))
                    {
                        fournisseur.IdProduitServices.Add(produitService);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde des produits et services", ex);
            }
        }

        public async Task UpdateProduitServiceData(ProduitServiceFormModel produitServiceFormModel, int idFournisseur, string email)
        {
            bool isEqual = true;
            var fournisseur = await _context.Fournisseurs.Include(f => f.IdProduitServices).FirstOrDefaultAsync(f => f.IdFournisseur == idFournisseur);
            if (fournisseur == null)
            {
                throw new Exception("Fournisseur non trouvé.");
            }
            try
            {
                string oldData = fournisseur.DetailSpecification;
                string newData = produitServiceFormModel.Message;
                string[] keyData = {"Détails et spécifications", "Produits et services"};
                var oldDict = new Dictionary<string, object> { { "Section", "Produits et services" } };
                var newDict = new Dictionary<string, object> { { "Section", "Produits et services" } };
                if (!oldData.Equals(newData))
                {
                    isEqual = false;
                    oldDict.Add(keyData[0], oldData);
                    newDict.Add(keyData[0], newData);
                }
                fournisseur.DetailSpecification = produitServiceFormModel.Message;

                var selectedProduitServiceIds = produitServiceFormModel.SousProduitSelected.Where(x => x.Value).Select(x => x.Key).ToList();
                var existingProduitServiceIds = fournisseur.IdProduitServices.Select(ps => ps.CodeUnspsc).ToList();
                var produitServicesToAdd = await _context.Produitservices.Where(ps => selectedProduitServiceIds.Contains(ps.CodeUnspsc) && !existingProduitServiceIds.Contains(ps.CodeUnspsc)).ToListAsync();
                var produitServicesToRemove = fournisseur.IdProduitServices.Where(ps => !selectedProduitServiceIds.Contains(ps.CodeUnspsc)).ToList();
                List<string> produitToAdd = new List<string>();
                List<string> produitToRemove = new List<string>();

                foreach (var produitService in produitServicesToAdd)
                {
                    isEqual = false;
                    fournisseur.IdProduitServices.Add(produitService);
                    produitToAdd.Add(produitService.CodeUnspsc + " - " + produitService.Description);
                }
                if (produitToAdd.Any()) newDict.Add(keyData[1], string.Join(":", produitToAdd));

                foreach (var produitService in produitServicesToRemove)
                {
                    isEqual = false;
                    fournisseur.IdProduitServices.Remove(produitService);
                    produitToRemove.Add(produitService.CodeUnspsc + " - " + produitService.Description);
                }
                if (produitToRemove.Any()) oldDict.Add(keyData[1], string.Join(":", produitToRemove));

                string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
                string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
                if (!isEqual)
                    await _historiqueService.ModifyEtat("Modifiée", fournisseur.IdFournisseur, email, null, oldJSON, newJSON);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la mise à jour des produits et services", ex);
            }
        }

    }
}
