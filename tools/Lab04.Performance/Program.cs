using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Lab02.Interfaces;

const int iterations = 1_000_000;
const int repeats = 9;
var circle = new Circle(5);
var rectangle = new Rectangle(4, 6);
var cube = new Cube(3);
var results = new[]
{
    Measure("Circle.GetArea", circle.GetArea),
    Measure("Rectangle.GetArea", rectangle.GetArea),
    Measure("Cube.GetVolume", cube.GetVolume)
};
var report = new
{
    Utc = DateTimeOffset.UtcNow,
    Framework = RuntimeInformation.FrameworkDescription,
    OS = RuntimeInformation.OSDescription,
    Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
    Iterations = iterations, Repeats = repeats,
    Results = results
};
string json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
string output = args.Length > 0 ? args[0] : "artifacts/performance.json";
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
File.WriteAllText(output, json + Environment.NewLine);

object Measure(string name, Func<double> operation)
{
    for (int i = 0; i < 10_000; i++) _ = operation();
    var times = new double[repeats];
    long allocated = 0;
    double checksum = 0;
    for (int repeat = 0; repeat < repeats; repeat++)
    {
        long before = GC.GetAllocatedBytesForCurrentThread();
        long started = Stopwatch.GetTimestamp();
        for (int i = 0; i < iterations; i++) checksum += operation();
        times[repeat] = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
        allocated += GC.GetAllocatedBytesForCurrentThread() - before;
    }
    Array.Sort(times);
    return new
    {
        Name = name, MedianMilliseconds = times[repeats / 2],
        NanosecondsPerOperation = times[repeats / 2] * 1_000_000 / iterations,
        AllocatedBytesPerOperation = (double)allocated / repeats / iterations,
        Checksum = checksum
    };
}
