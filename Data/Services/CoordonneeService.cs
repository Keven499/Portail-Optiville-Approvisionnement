using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;

namespace Portail_OptiVille.Data.Services
{
    public class CoordonneeService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;

        public CoordonneeService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        public async Task SaveCoordonneeData(CoordonneeFormModel coordonneeFormModelDto)
        {
            var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
            var coordonnee = new Coordonnee
            {
                NoCivique = coordonneeFormModelDto.NoEntreprise,
                Rue = coordonneeFormModelDto.RueEntreprise,
                Bureau = coordonneeFormModelDto.BureauEntreprise,
                Ville = coordonneeFormModelDto.VilleEntreprise,
                Province = coordonneeFormModelDto.ProvinceEntreprise,
                CodePostal = coordonneeFormModelDto.CodePostalEntreprise.Replace(" ", ""),
                CodeRegionAdministrative = coordonneeFormModelDto.RegionAdmEntreprise?.Substring(coordonneeFormModelDto.RegionAdmEntreprise.IndexOf('(')
                                                                                     + 1, coordonneeFormModelDto.RegionAdmEntreprise.IndexOf(')')
                                                                                     - coordonneeFormModelDto.RegionAdmEntreprise.IndexOf('(') - 1),
                RegionAdministrative = coordonneeFormModelDto.RegionAdmEntreprise?.Substring(coordonneeFormModelDto.RegionAdmEntreprise.IndexOf(' ') + 1),
                SiteInternet = coordonneeFormModelDto.SiteWebEntreprise,
                Fournisseur = lastFournisseurId
            };

            try
            {
                _context.Coordonnees.Add(coordonnee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde des coordonnées", ex);
            }

            var telephones = new List<Telephone>();
            var lastCoordonneId = await _context.Coordonnees.MaxAsync(f => (int?)f.IdCoordonnee);
            foreach (var telephoneFromList in coordonneeFormModelDto.PhoneList)
            {
                var telephone = new Telephone
                {
                    Type = telephoneFromList.TypeTelEntreprise,
                    NumTelephone = telephoneFromList.NoTelEntreprise,
                    Poste = telephoneFromList.PosteTelEntreprise,
                    Contact = null,
                    Coordonnee = lastCoordonneId
                };

                telephones.Add(telephone);
            }

            try
            {
                await _context.Telephones.AddRangeAsync(telephones);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde des téléphones dans coordonnées", ex);
            }
        }

        public async Task UpdateCoordonneeData(CoordonneeFormModel coordonneeFormModel, string email)
        {
            bool isEqual = true;
            string? codeRegion = coordonneeFormModel.RegionAdmEntreprise?.Substring(coordonneeFormModel.RegionAdmEntreprise.IndexOf('(')
                                                                                     + 1, coordonneeFormModel.RegionAdmEntreprise.IndexOf(')')
                                                                                     - coordonneeFormModel.RegionAdmEntreprise.IndexOf('(') - 1);
            string? regionAdm = coordonneeFormModel.RegionAdmEntreprise?.Substring(coordonneeFormModel.RegionAdmEntreprise.IndexOf(' ') + 1);
            var coordonnee = await _context.Coordonnees.FindAsync(coordonneeFormModel.IdCoordonnee);
            string[] oldData = {coordonnee.NoCivique, coordonnee.Rue, coordonnee.Bureau, 
                                coordonnee.Ville, coordonnee.Province, coordonnee.CodePostal, 
                                coordonnee.CodeRegionAdministrative, coordonnee.RegionAdministrative, coordonnee.SiteInternet};
            string[] newData = {coordonneeFormModel.NoEntreprise, coordonneeFormModel.RueEntreprise, coordonneeFormModel.BureauEntreprise,
                                coordonneeFormModel.VilleEntreprise, coordonneeFormModel.ProvinceEntreprise, coordonneeFormModel.CodePostalEntreprise,
                                codeRegion, regionAdm, coordonneeFormModel.SiteWebEntreprise};
            string[] keyData = {"No Civique", "Rue", "Bureau",
                                "Ville", "Province", "Code Postal", 
                                "Code région administrative", "Région administrative", "Site"};
            var oldDict = new Dictionary<string, object> { { "Section", "Finance" } };
            var newDict = new Dictionary<string, object> { { "Section", "Finance" } };
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
                await _historiqueService.ModifyEtat("Modifiée", (int)coordonnee.Fournisseur, email, null, oldJSON, newJSON);

            coordonnee.NoCivique = coordonneeFormModel.NoEntreprise;
            coordonnee.Rue = coordonneeFormModel.RueEntreprise;
            coordonnee.Bureau = coordonneeFormModel.BureauEntreprise;
            coordonnee.Ville = coordonneeFormModel.VilleEntreprise;
            coordonnee.Province = coordonneeFormModel.ProvinceEntreprise;
            coordonnee.CodePostal = coordonneeFormModel.CodePostalEntreprise;
            coordonnee.CodeRegionAdministrative = codeRegion;
            coordonnee.RegionAdministrative = regionAdm;
            coordonnee.SiteInternet = coordonneeFormModel.SiteWebEntreprise;

            foreach (var telephoneFromList in coordonneeFormModel.PhoneList)
            {
                var telephone = await _context.Telephones.FindAsync(telephoneFromList.IdTelephone);
                telephone.NumTelephone = telephoneFromList.NoTelEntreprise; 
                telephone.Type = telephoneFromList.TypeTelEntreprise; 
                telephone.Poste = telephoneFromList.PosteTelEntreprise; 
                _context.Telephones.Update(telephone);
            }

            _context.Coordonnees.Update(coordonnee);
            await _context.SaveChangesAsync();
        }
    }
}