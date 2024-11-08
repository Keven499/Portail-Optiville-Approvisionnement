namespace Portail_OptiVille.Data.Utilities
{
    using System.IO.Compression;
    public class FileManager
    {
        // Méthode pour dézipper un fichier
        public void UnzipFile(string zipPath, string extractPath)
        {
            // Si le répertoire n'existe pas, on le crée
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        // Méthode pour déplacer un fichier
        public void MoveFile(string sourcePath, string destinationPath)
        {
            File.Move(sourcePath, destinationPath);
        }

        // Méthode pour supprimer un fichier
        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        // Méthode pour supprimer un répertoire
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        // Méthode pour renommer un fichier
        public void RenameFile(string path, string newName)
        {
            File.Move(path, Path.Combine(Path.GetDirectoryName(path), newName));
        }
    }
}
