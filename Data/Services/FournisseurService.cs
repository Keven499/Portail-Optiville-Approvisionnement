using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.Services
{
    public class FournisseurService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public FournisseurService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
        }

        public async Task SaveFournisseurData(ProduitServiceFormModel produitServiceFormModelDto)
        {
            var fournisseur = new Fournisseur
            {
                DateCreation = DateTime.Now
            };

            _context.Fournisseurs.Add(fournisseur);
            await _context.SaveChangesAsync();
        }
    }
}
