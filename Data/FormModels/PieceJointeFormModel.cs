using Microsoft.AspNetCore.Components.Forms;
using Portail_OptiVille.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Portail_OptiVille.Data.FormModels
{
    public class PieceJointeFormModel
    {
        public IBrowserFile? Fichier { get; set; }

        public List<Fichier> ListFichiers = new List<Fichier>();

        public Dictionary<string, Stream> FileStreams { get; set; } = new Dictionary<string, Stream>();
    }
}
