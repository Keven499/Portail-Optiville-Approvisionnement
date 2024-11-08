namespace Portail_OptiVille.Data.Utilities
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class Modele
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Objet { get; set; }
        public string Message { get; set; }
        private List<Modele> ListModele { get; set; } = new List<Modele>();

        public Modele(int id, string nom, string objet, string message)
        {
            Id = id;
            Nom = nom;
            Objet = objet;
            Message = message;
        }
        
        public void AddListModele(Modele modele)
        {
            ListModele.Add(modele);
        }

        public void RemoveListModele(Modele modele)
        {
            ListModele.Remove(modele);
        }

        public static async Task<List<Modele>> LoadFromJsonAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var modeleJson = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                var modeles = JsonSerializer.Deserialize<List<Modele>>(modeleJson, options);
                if (modeles != null)
                {
                    return modeles;
                }
                else
                {
                    throw new InvalidDataException($"Failed to deserialize JSON from: {filePath}");
                }
            }
            throw new FileNotFoundException($"Configuration file not found: {filePath}");
        }

    }
}
