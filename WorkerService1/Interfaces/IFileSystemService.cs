namespace WorkerService1.Interfaces;

public interface IFileSystemService
{
    DirectoryInfo CreateDirectory(string path);
    bool DirectoryExistis(string path);

    Task WriteAllTextToFile(
        string fileName,
        string text);
}