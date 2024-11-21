using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Portail_OptiVille.Data.Services
{
    public class IdentificationService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;
        public IdentificationService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        private string HashPassword(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public async Task SaveIdentificationData(IdenticationFormModel identificationFormModelDto)
        {
            var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
            try
            {
                var identification = new Identification
                {
                    Neq = identificationFormModelDto.NEQ,
                    NomEntreprise = identificationFormModelDto.NomEntreprise,
                    AdresseCourriel = identificationFormModelDto.CourrielEntreprise,
                    MotDePasse = HashPassword(identificationFormModelDto.MotDePasse),
                    Fournisseur = lastFournisseurId
                };

                _context.Identifications.Add(identification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde de l'identification", ex);
            }
        }

        public async Task UpdateIdentificationData(IdenticationFormModel identicationFormModel, string email)
        {
            bool isEqual = true;
            var identification = await _context.Identifications.FindAsync(identicationFormModel.IdIdentification);
            string[] oldData = {identification.Neq, identification.NomEntreprise, identification.AdresseCourriel};
            string[] newData = {identicationFormModel.NEQ, identicationFormModel.NomEntreprise, identicationFormModel.CourrielEntreprise};
            string[] keyData = {"NEQ", "Nom de l'entreprise", "Adresse courriel"};
            var oldDict = new Dictionary<string, object> { { "Section", "Identification" } };
            var newDict = new Dictionary<string, object> { { "Section", "Identification" } };
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
                await _historiqueService.ModifyEtat("Modifiée", (int)identification.Fournisseur, email, null, oldJSON, newJSON);
            
            identification.Neq = identicationFormModel.NEQ;
            identification.NomEntreprise = identicationFormModel.NomEntreprise;
            identification.AdresseCourriel = identicationFormModel.CourrielEntreprise;
            identification.MotDePasse = identicationFormModel.MotDePasse;
            
            _context.Identifications.Update(identification);
            await _context.SaveChangesAsync();
        }
    }
}
