namespace Vidmake.src
{
    public static class PathChecking
    {
        public static bool CanCreateFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            try
            {
                string fileName = Path.GetFileName(path);
                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return false;

                string? dir = Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();
                if (!Directory.Exists(dir)) return false;

                string tempFile = Path.Combine(dir, Path.GetRandomFileName());
                using (FileStream fs = File.Create(tempFile)) { }
                File.Delete(tempFile);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}