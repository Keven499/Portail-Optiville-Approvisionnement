using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.Services
{
    public class GestionUserService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public GestionUserService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
        }

        public async Task UpdateUserRole(string courriel, string role)
        {
            var userToUpdate = await _context.Employes.FindAsync(courriel);
            if (userToUpdate != null)
            {
                // Update the role field to "Aucun"
                userToUpdate.Role = role;

                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        public async Task UpdateRoleOfUsers(List<string> employesCourriel, List<string> employesRole)
        {
            if (employesCourriel.Count != employesRole.Count)
            {
                throw new ArgumentException("The lists employesCourriel and employesRole must have the same number of elements.");
            }

            for (int i = 0; i < employesCourriel.Count; i++)
            {
                string courriel = employesCourriel[i];
                string role = employesRole[i];
                var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Courriel == courriel);

                if (employe != null)
                {
                    employe.Role = role;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
