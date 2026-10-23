using Lab02.Interfaces;
using Moq;
using Xunit;

namespace Lab04.Tests;

public sealed class LoggingTests
{
    [Fact]
    public void ConsoleLogger_WritesPrefix()
    {
        using var console = new ConsoleCapture();
        new ConsoleLogger().Log("Hello");
        Assert.Equal("LOG: Hello" + Environment.NewLine, console.Output.ToString());
    }

    [Fact]
    public void DoWork_LogsStartThenCompletion()
    {
        var logger = new Mock<ILogger>(MockBehavior.Strict);
        var order = new MockSequence();
        logger.InSequence(order).Setup(x => x.Log("Work started"));
        logger.InSequence(order).Setup(x => x.Log("Work completed"));
        Demo.DoWork(logger.Object);
        logger.Verify(x => x.Log("Work started"), Times.Once);
        logger.Verify(x => x.Log("Work completed"), Times.Once);
        logger.VerifyNoOtherCalls();
    }

    [Theory, InlineData("Work started"), InlineData("Work completed")]
    public void DoWork_DoesNotHideLoggingErrors(string message)
    {
        var logger = new Mock<ILogger>();
        var failure = new IOException("Disk full");
        logger.Setup(x => x.Log(message)).Throws(failure);
        Assert.Same(failure, Assert.Throws<IOException>(() => Demo.DoWork(logger.Object)));
        if (message == "Work started")
            logger.Verify(x => x.Log("Work completed"), Times.Never);
    }

    [Fact]
    public void FileLogger_UsesInjectedStorageInOrder()
    {
        string path = Path.Combine(Path.GetTempPath(), "lab04-mock", "work.log");
        var storage = new Mock<ILogStorage>(MockBehavior.Strict);
        var order = new MockSequence();
        storage.InSequence(order).Setup(x => x.GetFullPath("work.log")).Returns(path);
        storage.InSequence(order).Setup(x => x.EnsureDirectory(Path.GetDirectoryName(path)!));
        storage.InSequence(order).Setup(x => x.AppendAllText(path, "Hello" + Environment.NewLine));
        new FileLogger("work.log", storage.Object).Log("Hello");
        storage.Verify(x => x.GetFullPath("work.log"), Times.Once);
        storage.Verify(x => x.EnsureDirectory(Path.GetDirectoryName(path)!), Times.Once);
        storage.Verify(x => x.AppendAllText(path, "Hello" + Environment.NewLine), Times.Once);
        storage.VerifyNoOtherCalls();
    }

    [Fact, Trait("Kind", "Integration")]
    public void FileLogger_CreatesDirectoryAndAppendsWithoutOverwriting()
    {
        string directory = Path.Combine(Path.GetTempPath(), "lab04-" + Guid.NewGuid());
        string path = Path.Combine(directory, "nested", "work.log");
        try
        {
            var logger = new FileLogger(path);
            logger.Log("First"); logger.Log("Second");
            Assert.Equal("First" + Environment.NewLine + "Second" + Environment.NewLine,
                File.ReadAllText(path));
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void FileLogger_PropagatesPathFailure()
    {
        var storage = new Mock<ILogStorage>();
        storage.Setup(x => x.GetFullPath("bad")).Throws<ArgumentException>();
        Assert.Throws<ArgumentException>(() => new FileLogger("bad", storage.Object));
        storage.Verify(x => x.EnsureDirectory(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void FileLogger_PropagatesDirectoryAndWriteFailures()
    {
        string path = Path.Combine(Path.GetTempPath(), "lab04-errors", "work.log");
        var storage = new Mock<ILogStorage>();
        storage.Setup(x => x.GetFullPath("work.log")).Returns(path);
        storage.Setup(x => x.EnsureDirectory(It.IsAny<string>())).Throws<IOException>();
        Assert.Throws<IOException>(() => new FileLogger("work.log", storage.Object));
        storage.Setup(x => x.EnsureDirectory(It.IsAny<string>()));
        storage.Setup(x => x.AppendAllText(path, It.IsAny<string>())).Throws<IOException>();
        var logger = new FileLogger("work.log", storage.Object);
        Assert.Throws<IOException>(() => logger.Log("Hello"));
    }
}
