namespace Lab02.Interfaces;

public interface ILogStorage
{
    string GetFullPath(string path);
    void EnsureDirectory(string path);
    void AppendAllText(string path, string content);
}

public sealed class PhysicalLogStorage : ILogStorage
{
    public string GetFullPath(string path) => Path.GetFullPath(path);
    public void EnsureDirectory(string path) => Directory.CreateDirectory(path);
    public void AppendAllText(string path, string content) =>
        File.AppendAllText(path, content);
}
