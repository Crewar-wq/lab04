using System.Globalization;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Lab04.Tests;

internal sealed class ConsoleCapture : IDisposable
{
    private readonly TextWriter _out = Console.Out;
    private readonly TextWriter _error = Console.Error;
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    public StringWriter Output { get; } = new();
    public StringWriter Error { get; } = new();
    public ConsoleCapture()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        Console.SetOut(Output);
        Console.SetError(Error);
    }
    public void Dispose()
    {
        Console.SetOut(_out);
        Console.SetError(_error);
        CultureInfo.CurrentCulture = _culture;
        Output.Dispose();
        Error.Dispose();
    }
}
