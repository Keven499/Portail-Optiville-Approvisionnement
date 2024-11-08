using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using System.Security.Cryptography;
using System.Text;

namespace Portail_OptiVille.Data.Services
{
    public class IdentificationService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public IdentificationService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
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

        public async Task UpdateIdentificationData(IdenticationFormModel identicationFormModel)
        {
            var identification = await _context.Identifications.FindAsync(identicationFormModel.IdIdentification);
            identification.Neq = identicationFormModel.NEQ;
            identification.NomEntreprise = identicationFormModel.NomEntreprise;
            identification.AdresseCourriel = identicationFormModel.CourrielEntreprise;
            identification.MotDePasse = identicationFormModel.MotDePasse;

            _context.Identifications.Update(identification);
            await _context.SaveChangesAsync();
        }
    }
}
