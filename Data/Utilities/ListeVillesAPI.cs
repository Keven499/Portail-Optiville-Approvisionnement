using System.Text.Json;
using static Portail_OptiVille.Data.Attributes.VilleApiResponse;

namespace Portail_OptiVille.Data.Attributes
{
    public class ListeVillesAPI
    {
        private readonly HttpClient _httpClient;

        public ListeVillesAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetVilles()
        {
            try
            {
                string sqlQuery = "SELECT DISTINCT \"munnom\" FROM \"19385b4e-5503-4330-9e59-f998f5918363\" ORDER BY \"munnom\"";
                var response = await _httpClient.GetAsync($"https://www.donneesquebec.ca/recherche/api/3/action/datastore_search_sql?sql={Uri.EscapeDataString(sqlQuery)}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<VilleApiResponse.CityApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var villes = apiResponse?.Result?.Records?.Select(record => record.Munnom).Distinct().ToList();
                    return villes ?? new List<string>();
                }
                else
                {
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                    return new List<string>(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
