namespace Vidmake.src
{
    public static class PathChecking
    {
        /// <summary>
        /// Checks if a directory path is valid and writable.
        /// </summary>
        public static bool CanUseDirectory(string dirPath)
        {
            if (string.IsNullOrWhiteSpace(dirPath))
                return false;

            try
            {
                if (dirPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return false;

                if (!Directory.Exists(dirPath))
                    return false;

                string tempFile = Path.Combine(dirPath, Path.GetRandomFileName());
                using (FileStream fs = File.Create(tempFile, 1, FileOptions.DeleteOnClose)) { }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a file path is valid and can be created.
        /// Internally uses CanUseDirectory to validate the directory.
        /// </summary>
        public static bool CanCreateFileAtPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return false;

                string fileName = Path.GetFileName(filePath);
                if (string.IsNullOrWhiteSpace(fileName) || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    return false;

                string? dir = Path.GetDirectoryName(filePath);
                if (string.IsNullOrWhiteSpace(dir))
                    dir = Directory.GetCurrentDirectory();

                return CanUseDirectory(dir);
            }
            catch
            {
                return false;
            }
        }
    }
}