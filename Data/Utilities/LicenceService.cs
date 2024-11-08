using System.Text.Json;
using static Portail_OptiVille.Data.Attributes.RBQApiResponse;

public class LicenceService
{
    private readonly HttpClient _httpClient;

    public LicenceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ValidateNumeroDeLicenceAsync(string noLicenceRBQ)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://www.donneesquebec.ca/recherche/api/3/action/datastore_search_sql?sql=SELECT%20*%20FROM%20%2232f6ec46-85fd-45e9-945b-965d9235840a%22%20WHERE%20%22Numero%20de%20licence%22%20=%20%27{noLicenceRBQ}%27LIMIT%201");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var responseNoLicence = apiResponse?.Result?.Records?.FirstOrDefault()?.NumeroDeLicence;
                return !string.IsNullOrEmpty(responseNoLicence);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return false;
    }
}
