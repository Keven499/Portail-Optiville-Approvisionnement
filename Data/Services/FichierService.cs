using Microsoft.EntityFrameworkCore;
using Portail_OptiVille.Data.FormModels;
using Portail_OptiVille.Data.Models;
using Newtonsoft.Json;

namespace Portail_OptiVille.Data.Services
{
    public class FichierService
    {
        private readonly A2024420517riGr1Eq6Context _context;
        private HistoriqueService _historiqueService;

        public FichierService(A2024420517riGr1Eq6Context context, HistoriqueService historiqueService)
        {
            _context = context;
            _historiqueService = historiqueService;
        }

        public async Task SaveFichierData(PieceJointeFormModel pieceJointeFormModelDto, IdenticationFormModel identificationFormModelDto)
        {
            var lastFournisseurId = await _context.Fournisseurs.MaxAsync(f => (int?)f.IdFournisseur);
            var fichiers = new List<Fichier>();

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", lastFournisseurId.ToString());
            Directory.CreateDirectory(folderPath);

            foreach (var fichierFromList in pieceJointeFormModelDto.ListFichiers)
            {
                try
                {
                    if (fichierFromList == null)
                    {
                        Console.WriteLine("File is null, skipping.");
                        continue;
                    }
                    var filePath = Path.Combine(folderPath, fichierFromList.Nom).ToLower();
                    if (File.Exists(filePath))
                    {
                        continue;
                    }
                    if (pieceJointeFormModelDto.FileStreams.TryGetValue(fichierFromList.Nom, out var fileStream))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Position = 0;
                            await fileStream.CopyToAsync(stream);
                        }

                    var fileExtension = Path.GetExtension(fichierFromList.Nom).ToLower();
                    Console.WriteLine(fichierFromList.Nom);
                    var fichier = new Fichier
                    {
                        // NE PAS METTRE L'EXTENSION DANS LE NOM
                        Nom = fichierFromList.Nom,
                        Type = fileExtension,
                        Taille = (int)fichierFromList.Taille,
                        DateCreation = DateTime.Now,
                        Path = Path.Combine("files", lastFournisseurId.ToString(), fichierFromList.Nom).ToLower(),
                        Fournisseur = lastFournisseurId
                    };
                    fichiers.Add(fichier); 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue lors de la sauvegarde du fichier {fichierFromList?.Nom}: {ex.Message}");
                    continue; 
                }
            }

            try
            {
                if (fichiers.Count > 0)
                {
                    await _context.Fichiers.AddRangeAsync(fichiers);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde des fichiers dans la base de données", ex);
            }
        }

        public async Task UpdateFichierData(PieceJointeFormModel pieceJointeFormModelDto, int fournisseurID, string email)
        {
            bool isEqual = true;
            string keyData = "Nom";
            var oldDict = new Dictionary<string, object> { { "Section", "Fichier" } };
            var newDict = new Dictionary<string, object> { { "Section", "Fichier" } };
            var fichiers = new List<Fichier>();
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", fournisseurID.ToString());
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var existingFichiers = await _context.Fichiers.Where(f => f.Fournisseur == fournisseurID).ToListAsync();
            var existingFileNames = existingFichiers.Select(f => f.Nom).ToHashSet();
            var filesToDelete = existingFichiers
            .Where(f => !pieceJointeFormModelDto.ListFichiers.Any(pf => pf.Nom == f.Nom))
            .ToList();
            int indexRem = 1;
            foreach (var fichierToDelete in filesToDelete)
            {
                try
                {
                    isEqual = false;
                    oldDict.Add(keyData + indexRem, fichierToDelete.Nom);
                    indexRem++;
                    var filePath = Path.Combine(folderPath, fichierToDelete.Nom).ToLower();
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    _context.Fichiers.Remove(fichierToDelete);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue lors de la suppression du fichier {fichierToDelete.Nom}: {ex.Message}");
                }
            }
            int indexAdd = 1;
            foreach (var fichierFromList in pieceJointeFormModelDto.ListFichiers)
            {
                try
                {
                    if (fichierFromList == null || string.IsNullOrEmpty(fichierFromList.Nom))
                    {
                        Console.WriteLine("File is null or has an empty name, skipping.");
                        continue;
                    }
                    isEqual = false;
                    Console.WriteLine(keyData  + indexAdd);

                    var filePath = Path.Combine(folderPath, fichierFromList.Nom).ToLower();
                    if (!existingFileNames.Contains(fichierFromList.Nom) &&
                        pieceJointeFormModelDto.FileStreams.TryGetValue(fichierFromList.Nom, out var fileStream))
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            fileStream.Position = 0;
                            await fileStream.CopyToAsync(stream);
                        }
                        var fileExtension = Path.GetExtension(fichierFromList.Nom).ToLower();
                        var fichier = new Fichier
                        {
                            Nom = fichierFromList.Nom,
                            Type = fileExtension,
                            Taille = (int)fichierFromList.Taille,
                            DateCreation = DateTime.Now,
                            Path = Path.Combine("files", fournisseurID.ToString(), fichierFromList.Nom).ToLower(),
                            Fournisseur = fournisseurID
                        };
                        newDict.Add(keyData + indexAdd, fichier.Nom);
                        indexAdd++;
                        _context.Fichiers.Add(fichier);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue lors de la sauvegarde du fichier {fichierFromList?.Nom}: {ex.Message}");
                }
            }
            try
            {
                string oldJSON = JsonConvert.SerializeObject(oldDict, Formatting.None);
                string newJSON = JsonConvert.SerializeObject(newDict, Formatting.None);
                if (!isEqual)
                    await _historiqueService.ModifyEtat("Modifiée", fournisseurID, email, null, oldJSON, newJSON);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Une erreur est survenue lors de la sauvegarde des fichiers dans la base de données", ex);
            }
        }

        public async Task DeleteAllFichiersData(List<Fichier> listFichiers)
        {
            foreach (var fichier in listFichiers)
            {
                try
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fichier.Path.Replace("\\", Path.DirectorySeparatorChar.ToString()));

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    else
                    {
                        Console.WriteLine($"File not found: {fullPath}, skipping deletion from server.");
                    }

                    _context.Fichiers.Remove(fichier);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Une erreur est survenue lors de la supression du fichier {fichier.Nom}: {ex.Message}");
                    continue;
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving changes to the database during bulk file deletion.", ex);
            }
        }

        public async Task DeleteOneFichierData(Fichier fichier)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fichier.Path.Replace("\\", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                else
                {
                    Console.WriteLine($"File not found: {fullPath}, skipping deletion from server.");
                }
                _context.Fichiers.Remove(fichier);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting the file {fichier.Nom} from the server or database.", ex);
            }
        }
    }
}
