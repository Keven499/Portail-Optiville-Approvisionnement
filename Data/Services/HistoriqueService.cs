using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.Services
{
    public class HistoriqueService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public HistoriqueService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
        }

        public async Task AddHistoriqueRefuser(int _idFournisseur, string _raisonRefus)
        {
            var historique = new Historique
            {
                EtatDemande = "Refusé",
                Fournisseur = _idFournisseur,
                RaisonRefus = _raisonRefus,
                DateEtatChanged = DateTime.UtcNow
            };

            _context.Historiques.Add(historique);
            await _context.SaveChangesAsync();
        }

        public async Task AddHistoriqueAccepter(int _idFournisseur)
        {
            var historique = new Historique
            {
                EtatDemande = "Accepté",
                Fournisseur = _idFournisseur,
                DateEtatChanged = DateTime.UtcNow
            };

            _context.Historiques.Add(historique);
            await _context.SaveChangesAsync();
        }

        public async Task AddHistoriqueEnAttente(int _idFournisseur)
        {
            if (_idFournisseur == -1)
            {
                _idFournisseur = await _context.Fournisseurs.MaxAsync(f => (int)f.IdFournisseur);
            }
            var historique = new Historique
            {
                EtatDemande = "En attente",
                Fournisseur = _idFournisseur,
                DateEtatChanged = DateTime.UtcNow
            };

            _context.Historiques.Add(historique);
            await _context.SaveChangesAsync();
        }
    }
}