namespace Portail_OptiVille.Data.Utilities
{
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class Config
    {
        public string? CourrielAppro { get; set; }
        public int DelaiRevision { get; set; }
        public int TailleMaxFichiers { get; set; }
        public string? CourrielFinance { get; set; }

        public static async Task<Config> LoadFromJsonAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var configJson = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<Config>(configJson)!;
            }
            throw new FileNotFoundException($"Configuration file not found: {filePath}");
        }
    }
}

