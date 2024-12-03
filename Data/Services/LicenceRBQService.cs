using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;

namespace Portail_OptiVille.Data.Services
{
    public class LicenceRBQService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;

        public LicenceRBQService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        public async Task SaveLicenceRBQData(LicenceRBQFormModel licenceRBQFormModelDto, string email, int? IdFournisseur)
        {
            var licenceRBQdata = await _context.Licencerbqs.FindAsync(licenceRBQFormModelDto.NumeroLicence);
            if (licenceRBQdata == null)
            {
                if (licenceRBQFormModelDto.NumeroLicence != null)
                {
                    if(IdFournisseur == -1)
                    {
                    IdFournisseur = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
                    }
                    var licenceRBQ = new Licencerbq
                    {
                        IdLicenceRbq = licenceRBQFormModelDto.NumeroLicence?.Replace("-", string.Empty),
                        Type = licenceRBQFormModelDto.TypeLicence,
                        Statut = licenceRBQFormModelDto.StatutLicence,
                        Fournisseur = IdFournisseur
                    };

                    try
                    {
                        await _context.SaveChangesAsync();
                        var selectedCategorieRBQIds = licenceRBQFormModelDto.SousCategoSelected.Where(x => x.Value).Select(x => x.Key).ToList();
                        var existingProduitServiceIds = licenceRBQ.IdCategorieRbqs.Select(crbq => crbq.CodeSousCategorie).ToList();
                        var CategorieRBQToAdd = await _context.Categorierbqs.Where(crbq => selectedCategorieRBQIds.Contains(crbq.CodeSousCategorie) && !existingProduitServiceIds.Contains(crbq.CodeSousCategorie)).ToListAsync();

                            foreach (var categorieRBQ in CategorieRBQToAdd)
                            {
                                licenceRBQ.IdCategorieRbqs.Add(categorieRBQ);
                            }
                            _context.Licencerbqs.Add(licenceRBQ);
                            await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Une erreur est survenue lors de la sauvegarde de la licence RBQ", ex);
                    }
                }
            }
            else
            {
                bool isEqual = true;
                string[] oldData = {licenceRBQdata.Type, licenceRBQdata.Statut, licenceRBQdata.IdLicenceRbq};
                string[] newData = {licenceRBQFormModelDto.TypeLicence, licenceRBQFormModelDto.StatutLicence, licenceRBQFormModelDto.NumeroLicence};
                string[] keyData = {"Numéro de licence", "Statut", "Type", "Catégories"};
                var oldDict = new Dictionary<string, object> { { "Section", "Licence RBQ" } };
                var newDict = new Dictionary<string, object> { { "Section", "Licence RBQ" } };
                for (int i = 0; i < oldData.Length; i++)
                {
                    if (!oldData[i].Equals(newData[i]))
                    {
                        isEqual = false;
                        oldDict.Add(keyData[i], oldData[i]);
                        newDict.Add(keyData[i], newData[i]);
                    }
                }
                
                licenceRBQdata.IdLicenceRbq = licenceRBQFormModelDto.NumeroLicence;
                licenceRBQdata.Statut = licenceRBQFormModelDto.StatutLicence;
                licenceRBQdata.Type = licenceRBQFormModelDto.TypeLicence;
                
                var selectedCategorieRBQIds = licenceRBQFormModelDto.SousCategoSelected.Where(x => x.Value).Select(x => x.Key).ToList();
                var existingProduitServiceIds = licenceRBQdata.IdCategorieRbqs.Select(crbq => crbq.CodeSousCategorie).ToList();
                var CategorieRBQToAdd = await _context.Categorierbqs.Where(crbq => selectedCategorieRBQIds.Contains(crbq.CodeSousCategorie) && !existingProduitServiceIds.Contains(crbq.CodeSousCategorie)).ToListAsync();
                var CategorieRBQToRemove = licenceRBQdata.IdCategorieRbqs.Where(crbq => !selectedCategorieRBQIds.Contains(crbq.CodeSousCategorie)).ToList();
                List<string> catToAdd = new List<string>();
                List<string> catToRemove = new List<string>();

                foreach (var categorieRBQ in CategorieRBQToAdd)
                {
                    isEqual = false;
                    licenceRBQdata.IdCategorieRbqs.Add(categorieRBQ);
                    catToAdd.Add(categorieRBQ.CodeSousCategorie + " - " + categorieRBQ.TravauxPermis);
                }
                if (catToAdd.Any()) newDict.Add(keyData[3], string.Join(":", catToAdd));

                foreach (var categorieRBQ in CategorieRBQToRemove)
                {
                    isEqual = false;
                    licenceRBQdata.IdCategorieRbqs.Remove(categorieRBQ);
                    catToRemove.Add(categorieRBQ.CodeSousCategorie + " - " + categorieRBQ.TravauxPermis);
                }
                if (catToRemove.Any()) oldDict.Add(keyData[3], string.Join(":", catToRemove));
                    
                string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
                string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
                if (!isEqual)
                    await _historiqueService.ModifyEtat("Modifiée", (int)licenceRBQdata.Fournisseur, email, null, oldJSON, newJSON);
                
                _context.Licencerbqs.Update(licenceRBQdata);
                await _context.SaveChangesAsync();
            }
        }
    }
}
