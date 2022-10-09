using WorkerService1.Interfaces;

namespace WorkerService1.Services
{
    public class FileSystemService : IFileSystemService
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool DirectoryExistis(string path)
        {
            return Directory.Exists(path);
        }

        public async Task WriteAllTextToFile(
            string fileName,
            string text)
        {
            await File.WriteAllTextAsync(
                fileName,
                text);
        }
    }
}
