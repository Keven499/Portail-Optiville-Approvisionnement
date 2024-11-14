using Microsoft.EntityFrameworkCore;
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

        // CETTE MÉTHODE DOIT ÊTRE APPELER À CHAQUE MODIFICATION D'ÉTAT
        public async Task ModifyEtat(string _etat, int _idFournisseur, string _raisonRefus, string? _modifiePar = null, string? _retirer = null, string? _modifier = null)
        {
            if (_idFournisseur == -1)
            {
                _idFournisseur = await _context.Fournisseurs.MaxAsync(f => (int)f.IdFournisseur);
            }
            var historique = new Historique
            {
                EtatDemande = _etat,
                Fournisseur = _idFournisseur,
                RaisonRefus = _raisonRefus,
                DateEtatChanged = DateTime.UtcNow
            };

             _context.Historiques.Add(historique);
            await _context.SaveChangesAsync();
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