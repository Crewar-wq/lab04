using Lab02.Interfaces;
using Xunit;

namespace Lab04.Tests;

public sealed class BaselineTests
{
    [Fact, Trait("Stage", "Baseline")]
    public void Circle_HasExpectedArea()
    {
        Assert.Equal(Math.PI * 25, new Circle(5).GetArea(), 10);
    }
}
