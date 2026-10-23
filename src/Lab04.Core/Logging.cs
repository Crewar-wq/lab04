namespace Lab02.Interfaces;

public interface ILogger { void Log(string message); }
public sealed class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"LOG: {message}");
}
public sealed class FileLogger : ILogger
{
    private readonly string _path;
    private readonly ILogStorage _storage;
    public FileLogger(string path) : this(path, new PhysicalLogStorage()) { }
    public FileLogger(string path, ILogStorage storage)
    {
        _storage = storage;
        _path = storage.GetFullPath(path);
        storage.EnsureDirectory(Path.GetDirectoryName(_path)!);
    }
    public void Log(string message) =>
        _storage.AppendAllText(_path, message + Environment.NewLine);
}

