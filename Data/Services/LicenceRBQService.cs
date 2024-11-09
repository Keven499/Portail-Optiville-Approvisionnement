using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.Services
{
    public class LicenceRBQService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public LicenceRBQService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
        }

        public async Task SaveLicenceRBQData(LicenceRBQFormModel licenceRBQFormModelDto)
        {
            var licenceRBQdata = await _context.Licencerbqs.FindAsync(licenceRBQFormModelDto.NumeroLicence);
            if(licenceRBQdata == null)
            {
                if (licenceRBQFormModelDto.NumeroLicence != null)
                {
                    var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
                    var licenceRBQ = new Licencerbq
                    {
                        IdLicenceRbq = licenceRBQFormModelDto.NumeroLicence?.Replace("-", string.Empty),
                        Type = licenceRBQFormModelDto.TypeLicence,
                        Statut = licenceRBQFormModelDto.StatutLicence,
                        Fournisseur = lastFournisseurId
                    };

                    try
                    {
                        _context.Licencerbqs.Add(licenceRBQ);
                        await _context.SaveChangesAsync();

                        foreach (var codeSousCategorie in licenceRBQFormModelDto.CodeSousCategorie)
                        {
                            var categorieRBq = await _context.Categorierbqs
                                .FirstOrDefaultAsync(p => p.CodeSousCategorie == codeSousCategorie);

                            if (categorieRBq != null && !licenceRBQ.IdCategorieRbqs.Contains(categorieRBq))
                            {
                                licenceRBQ.IdCategorieRbqs.Add(categorieRBq);
                            }
                        }

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
                    licenceRBQdata.Fournisseur = licenceRBQdata.Fournisseur;
                    licenceRBQdata.IdLicenceRbq = licenceRBQFormModelDto.NumeroLicence;
                    licenceRBQdata.Statut = licenceRBQFormModelDto.StatutLicence;
                    licenceRBQdata.Type = licenceRBQFormModelDto.TypeLicence;

                    var selectedCategorieRBQIds = licenceRBQFormModelDto.SousCategoSelected.Where(x => x.Value).Select(x => x.Key).ToList();
                    var existingProduitServiceIds = licenceRBQdata.IdCategorieRbqs.Select(crbq => crbq.CodeSousCategorie).ToList();
                    var CategorieRBQToAdd = await _context.Categorierbqs.Where(crbq => selectedCategorieRBQIds.Contains(crbq.CodeSousCategorie) && !existingProduitServiceIds.Contains(crbq.CodeSousCategorie)).ToListAsync();
                    var CategorieRBQToRemove = licenceRBQdata.IdCategorieRbqs.Where(crbq => !selectedCategorieRBQIds.Contains(crbq.CodeSousCategorie)).ToList();

                    foreach (var categorieRBQ in CategorieRBQToAdd)
                    {
                        licenceRBQdata.IdCategorieRbqs.Add(categorieRBQ);
                    }

                    foreach (var categorieRBQ in CategorieRBQToRemove)
                    {
                        licenceRBQdata.IdCategorieRbqs.Remove(categorieRBQ);
                    }

                    _context.Licencerbqs.Update(licenceRBQdata);
                    await _context.SaveChangesAsync();
            }
        }
    }
}
