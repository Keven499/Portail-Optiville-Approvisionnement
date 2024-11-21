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
        public async Task ModifyEtat(string _etat, int _idFournisseur, string _modifiePar, string? _raisonRefus = null, string? _retirer = null, string? _ajouter = null)
        {
            Console.WriteLine(_retirer);
            Console.WriteLine(_ajouter);
            if (_idFournisseur == -1)
            {
                _idFournisseur = await _context.Fournisseurs.MaxAsync(f => (int)f.IdFournisseur);
            }
            var historique = new Historique
            {
                EtatDemande = _etat,
                Fournisseur = _idFournisseur,
                RaisonRefus = _raisonRefus,
                ModifiePar = _modifiePar,
                Retirer = _retirer,
                Ajouter = _ajouter,
                DateEtatChanged = DateTime.Now
            };

             _context.Historiques.Add(historique);
            await _context.SaveChangesAsync();
        }
    }
}