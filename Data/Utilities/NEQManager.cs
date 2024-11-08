using System.IO.Compression;
using System.Security.Policy;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;


namespace Portail_OptiVille.Data.Utilities
{
    public class Entreprise
    {
        public string NEQ { get; set; }
        public string ADR_DOMCL_LIGN1_ADR { get; set; }
        public string ADR_DOMCL_LIGN2_ADR { get; set; }
        public string ADR_DOMCL_LIGN3_ADR { get; set; }
        public string ADR_DOMCL_LIGN4_ADR { get; set; }
    }

    public class NEQManager
    {
        public static HashSet<string> listeEntreprise = new HashSet<string>();

        public static async Task ManageZIP(string zipPath, string? outputPath)
        {
            try
            {

                if (!Directory.Exists("Data\\Downloads"))
                {
                    Directory.CreateDirectory("Data\\Downloads");
                }

                // 1. Unzip le ZIP
                if (outputPath == null)
                {
                    outputPath = "Data\\Downloads";
                }
                ZipFile.ExtractToDirectory(zipPath, outputPath);

                // 2. Supprimer les fichiers non nécessaires
                string[] filesToDelete = new string[]
                {
            "ContinuationsTransformations.csv", "DomaineValeur.csv",
            "Etablissements.csv", "FusionScissions.csv", "Nom.csv", "entreprises.zip"
                };
                foreach (var file in filesToDelete)
                {
                    var filePath = Path.Combine(outputPath, file);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                // 4. Lecture et transformation du CSV en HashSet
                string csvFilePath = Path.Combine(outputPath, "Entreprise.csv");
                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    var records = csv.GetRecords<Entreprise>().ToList();

                    foreach (var record in records)
                    {
                        string combinedAdresse = $"{record.NEQ}|{record.ADR_DOMCL_LIGN1_ADR}|{record.ADR_DOMCL_LIGN2_ADR}|{record.ADR_DOMCL_LIGN3_ADR}|{record.ADR_DOMCL_LIGN4_ADR}";
                        listeEntreprise.Add(combinedAdresse);
                    }
                }

                // 6. Afficher en console le nombre d'entreprises
                Console.WriteLine($"Nombre d'entreprises: {listeEntreprise.Count}");
            }
            catch (Exception e)
            {
                foreach (var file in Directory.GetFiles("Data\\Downloads"))
                {
                    File.Delete(file);
                }
            }


        }
    }
}
