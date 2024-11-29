using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

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
            string[] oldData = {coordonnee.NoCivique, coordonnee.Rue, coordonnee.Bureau ?? "",  
                                coordonnee.Ville, coordonnee.Province, coordonnee.CodePostal, 
                                coordonnee.CodeRegionAdministrative, coordonnee.RegionAdministrative, coordonnee.SiteInternet ?? ""};
            string[] newData = {coordonneeFormModel.NoEntreprise, coordonneeFormModel.RueEntreprise, coordonneeFormModel.BureauEntreprise ?? "",
                                coordonneeFormModel.VilleEntreprise, coordonneeFormModel.ProvinceEntreprise, coordonneeFormModel.CodePostalEntreprise,
                                codeRegion, regionAdm, coordonneeFormModel.SiteWebEntreprise ?? ""};
            string[] keyData = {"No Civique", "Rue", "Bureau",
                                "Ville", "Province", "Code Postal", 
                                "Code région administrative", "Région administrative", "Site"
                                , "Type de téléphone", "Numéro de téléphone", "Poste"};
            var oldDict = new Dictionary<string, object> { { "Section", "Coordonnées" } };
            var newDict = new Dictionary<string, object> { { "Section", "Coordonnées" } };
            List<Telephone> oldTelephones = await _context.Telephones.Where(c => c.Coordonnee == coordonneeFormModel.IdCoordonnee).ToListAsync();
            List<TelephoneFormModel> newDataTelephones = coordonneeFormModel.PhoneList;
            List<string> catToAddType = new List<string>();
            List<string> catToAddNum = new List<string>();
            List<string> catToAddPoste = new List<string>();
            
            List<string> catToRemoveType = new List<string>();
            List<string> catToRemoveNum = new List<string>();
            List<string> catToRemovePoste = new List<string>();
            List<Telephone> missingPhones = oldTelephones
                .Where(c => !newDataTelephones
                    .Any(modelTelephone => modelTelephone.IdTelephone == c.IdTelephone))
                .ToList();
            foreach (var missingPhone in missingPhones)
            {
                var phone = await _context.Telephones
                    .Where(t => t.Contact == missingPhone.IdTelephone)
                    .FirstOrDefaultAsync();
                isEqual = false;
                _context.Telephones.Remove(missingPhone);
                catToRemoveType.Add(missingPhone.Type);
                catToRemoveNum.Add(missingPhone.NumTelephone);
                catToRemovePoste.Add(missingPhone.Poste);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors l'effacement d'un téléphone", ex);
            }
            for (int i = 0; i < oldData.Length; i++)
            {
                if (!oldData[i].Equals(newData[i]))
                {
                    isEqual = false;
                    oldDict.Add(keyData[i], oldData[i]);
                    newDict.Add(keyData[i], newData[i]);
                }
            }
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
                isEqual = false;
                var existingTelephone = await _context.Telephones.FindAsync(telephoneFromList.IdTelephone);
                if (existingTelephone != null)
                {
                    if (existingTelephone.Type != telephoneFromList.TypeTelEntreprise) { catToAddType.Add(telephoneFromList.TypeTelEntreprise);
                                                                                                catToRemoveType.Add(existingTelephone.Type); }
                    if (existingTelephone.NumTelephone != telephoneFromList.NoTelEntreprise) { catToAddNum.Add(telephoneFromList.NoTelEntreprise);
                                                                                        catToRemoveNum.Add(existingTelephone.NumTelephone); }    
                    if (existingTelephone.Poste != telephoneFromList.PosteTelEntreprise) { catToAddPoste.Add(telephoneFromList.PosteTelEntreprise);
                                                                                catToRemovePoste.Add(existingTelephone.Poste); } 
                    existingTelephone.NumTelephone = telephoneFromList.NoTelEntreprise; 
                    existingTelephone.Type = telephoneFromList.TypeTelEntreprise; 
                    existingTelephone.Poste = telephoneFromList.PosteTelEntreprise; 
                    try
                    {
                        _context.Telephones.Update(existingTelephone);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Une erreur est survenue lors de la mise à jour du contact", ex);
                    }
                    
                }
                else
                {
                    isEqual = false;
                    var lastTelephoneId = await _context.Telephones.MaxAsync(f => (int?)f.IdTelephone);
                    var telephone = new Telephone
                    {
                        NumTelephone = telephoneFromList.NoTelEntreprise,
                        Poste = telephoneFromList.PosteTelEntreprise,
                        Type = telephoneFromList.TypeTelEntreprise,
                        Coordonnee = coordonneeFormModel.IdCoordonnee, 
                        IdTelephone = (int)lastTelephoneId + 1
                    };
                    catToAddType.Add(telephone.Type);
                    catToAddNum.Add(telephone.NumTelephone);
                    catToAddPoste.Add(telephone.Poste);
                    try
                    {
                        _context.Telephones.Add(telephone);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Une erreur est survenue lors de la sauvegarde du téléphone", ex);
                    }
                }    
            }
            if (!catToRemoveType.IsNullOrEmpty()) oldDict.Add(keyData[9], string.Join(":", catToRemoveType));
            if (!catToRemoveNum.IsNullOrEmpty()) oldDict.Add(keyData[10], string.Join(":", catToRemoveNum));
            if (!catToRemovePoste.IsNullOrEmpty()) oldDict.Add(keyData[11], string.Join(":", catToRemovePoste));

            if (!catToAddType.IsNullOrEmpty()) newDict.Add(keyData[9], string.Join(":", catToAddType));
            if (!catToAddNum.IsNullOrEmpty()) newDict.Add(keyData[10], string.Join(":", catToAddNum));
            if (!catToAddPoste.IsNullOrEmpty()) newDict.Add(keyData[11], string.Join(":", catToAddPoste));
            string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
            string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
            if (!isEqual)
                await _historiqueService.ModifyEtat("Modifiée", (int)coordonnee.Fournisseur, email, null, oldJSON, newJSON);
            _context.Coordonnees.Update(coordonnee);    
            await _context.SaveChangesAsync();
        }
    }
}