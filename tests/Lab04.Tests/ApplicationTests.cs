using Lab02.Interfaces;
using Lab04.App;
using Moq;
using Xunit;

namespace Lab04.Tests;

public sealed class ApplicationTests
{
    private static Mock<ILogStorage> Storage()
    {
        var storage = new Mock<ILogStorage>();
        storage.Setup(x => x.GetFullPath(It.IsAny<string>()))
            .Returns(Path.Combine(Path.GetTempPath(), "lab04-app", "work.log"));
        return storage;
    }

    [Theory, InlineData(true), InlineData(false)]
    public void Run_UsesRequestedOrDefaultPath(bool custom)
    {
        using var console = new ConsoleCapture();
        var storage = Storage();
        Assert.Equal(0, Program.Run(custom ? ["custom.log"] : [], storage.Object));
        storage.Verify(x => x.GetFullPath(custom ? "custom.log" : "artifacts/work.log"), Times.Once);
        storage.Verify(x => x.AppendAllText(It.IsAny<string>(), "Work started" + Environment.NewLine), Times.Once);
        storage.Verify(x => x.AppendAllText(It.IsAny<string>(), "Work completed" + Environment.NewLine), Times.Once);
        Assert.Contains("File log written", console.Output.ToString());
        Assert.Equal("", console.Error.ToString());
    }

    [Fact]
    public void Run_ReturnsFailureAndReportsIoError()
    {
        using var console = new ConsoleCapture();
        var storage = Storage();
        storage.Setup(x => x.AppendAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new IOException("Disk full"));
        Assert.Equal(1, Program.Run([], storage.Object));
        Assert.Equal("I/O error: Disk full" + Environment.NewLine, console.Error.ToString());
        Assert.DoesNotContain("File log written", console.Output.ToString());
    }

    [Fact]
    public void Run_DoesNotSwallowUnrelatedErrors()
    {
        using var console = new ConsoleCapture();
        var storage = Storage();
        storage.Setup(x => x.GetFullPath(It.IsAny<string>())).Throws<ArgumentException>();
        Assert.Throws<ArgumentException>(() => Program.Run([], storage.Object));
    }

    [Fact, Trait("Kind", "Integration")]
    public void Main_ComposesRealStorageAndRunsApplication()
    {
        using var console = new ConsoleCapture();
        string directory = Path.Combine(Path.GetTempPath(), "lab04-main-" + Guid.NewGuid());
        string path = Path.Combine(directory, "work.log");
        try
        {
            Assert.Equal(0, Program.Main([path]));
            Assert.Equal("Work started" + Environment.NewLine + "Work completed" +
                Environment.NewLine, File.ReadAllText(path));
            Assert.Contains("Cube volume=27.00", console.Output.ToString());
            Assert.Contains("Cash payment: 250.00", console.Output.ToString());
        }
        finally { Directory.Delete(directory, true); }
    }
}
