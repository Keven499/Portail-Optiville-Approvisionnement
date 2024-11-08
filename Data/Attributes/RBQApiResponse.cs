using System.Text.Json.Serialization;

namespace Portail_OptiVille.Data.Attributes
{
    public class RBQApiResponse
    {
        public class ApiResponse
        {
            public Result? Result { get; set; }
        }

        public class Result
        {
            public List<RBQRecord>? Records { get; set; }
        }

        public class RBQRecord
        {
            [JsonPropertyName("Numero de licence")]
            public string? NumeroDeLicence { get; set; }
        }

    }
}