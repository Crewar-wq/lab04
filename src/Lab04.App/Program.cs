using System.Globalization;
using Lab02.Interfaces;

namespace Lab04.App;

public static class Program
{
    public static int Main(string[] args) => Run(args, new PhysicalLogStorage());

    public static int Run(string[] args, ILogStorage storage)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            string path = args.Length > 0 ? args[0] : "artifacts/work.log";
            DemoApplication.Run(path, storage);
            return 0;
        }
        catch (IOException error)
        {
            Console.Error.WriteLine($"I/O error: {error.Message}");
            return 1;
        }
    }
}
