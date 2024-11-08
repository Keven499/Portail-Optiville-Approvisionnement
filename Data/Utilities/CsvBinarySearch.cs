namespace Portail_OptiVille.Data.Utilities{

    using System;
    using System.Collections.Generic;
    using System.IO;

    public class CsvBinarySearch
    {
        // Charger le fichier CSV et trier les lignes selon la colonne NEQ
        public static List<string[]> LoadAndSortCsv(string filePath)
        {
            var csvData = new List<string[]>();

            using (var reader = new StreamReader(filePath))
            {
                // Lire l'en-tête et ignorer
                string headerLine = reader.ReadLine();
                
                // Lire les lignes du CSV
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    
                    // Ajouter la ligne à la liste
                    csvData.Add(values);
                }
            }

            // Trier les données par NEQ (première colonne, index 0)
            //csvData.Sort((x, y) => string.Compare(x[0], y[0], StringComparison.Ordinal));

            return csvData;
        }

        // Algorithme de recherche binaire sur la colonne NEQ
        public static int BinarySearch(List<string[]> sortedData, string targetNEQ)
        {
            int left = 0;
            int right = sortedData.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                string midNEQ = sortedData[mid][0];

                int comparison = string.Compare(midNEQ, targetNEQ, StringComparison.Ordinal);

                if (comparison == 0)
                {
                    return mid; // NEQ trouvé
                }
                else if (comparison < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1; // NEQ non trouvé
        }
    }
} 