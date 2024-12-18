using System.Text.Json.Serialization;

namespace Portail_OptiVille.Data.Attributes
{
    public class VilleApiResponse
    {
        public class CityApiResponse
        {
            public CityResult? Result { get; set; }
        }

        public class CityResult
        {
            public List<CityRecord>? Records { get; set; }
        }

        public class CityRecord
        {
            [JsonPropertyName("munnom")] // Assuming the API returns "munnom" for city names
            public string? Munnom { get; set; }
            [JsonPropertyName("regadm")] // Add this line to map the "regadm" field from the API response
            public string? Regadm { get; set; }
        }
    }
}
