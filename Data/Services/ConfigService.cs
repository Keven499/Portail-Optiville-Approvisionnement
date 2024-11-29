using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;

namespace Portail_OptiVille.Data.Services
{
    public class ConfigService
    {
        private readonly A2024420517riGr1Eq6Context _context;

        public ConfigService(A2024420517riGr1Eq6Context context)
        {
            _context = context;
        }

        public async Task ModifierConfig(ConfigFormModel configApproNew)
        {
            var allConfigsOld = await _context.Configappros.SingleAsync();
            allConfigsOld.CourrielAppro = configApproNew.CourrielAppro;
            allConfigsOld.CourrielFinance = configApproNew.CourrielFinance;
            allConfigsOld.DelaiRevision = configApproNew.DelaiBeforeRevision;
            allConfigsOld.TailleMaxFichiers = configApproNew.MaxFileSize;
            _context.Configappros.Update(allConfigsOld);
            await _context.SaveChangesAsync();
        }
    }
}