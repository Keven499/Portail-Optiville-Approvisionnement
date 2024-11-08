namespace Portail_OptiVille.Data.Utilities
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class DownloadService
    {
        private readonly HttpClient _httpClient;

        public DownloadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // Sert à aller télécharger un fichier à partir d'une URL et de le sauvegarder dans un répertoire (outputPath)
        public async Task<string> DownloadFileAsync(string url, string outputPath)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko ");
            request.Headers.Add("Referer", "https://www.registreentreprises.gouv.qc.ca/");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");


            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(outputPath, content);

            return outputPath;
        }

    }

}
