using Lab02.Interfaces;
using Moq;
using Xunit;

namespace Lab04.Tests;

public sealed class ShapesTests
{
    public static TheoryData<double> InvalidSizes =>
        new() { 0, -1, double.NaN, double.PositiveInfinity, double.NegativeInfinity };

    [Fact]
    public void Point_MoveSetsAbsoluteCoordinates()
    {
        var point = new Point(1, 2);
        Assert.Equal((1, 2), (point.X, point.Y));
        ((IMovable)point).Move(-10, 20);
        Assert.Equal((-10, 20), (point.X, point.Y));
        Assert.Equal("Point(-10, 20)", point.ToString());
    }

    [Fact]
    public void Shapes_CalculateAreaPerimeterAndVolume()
    {
        var circle = new Circle(5);
        var rectangle = new Rectangle(4, 6);
        var cube = new Cube(3);
        Assert.Equal(5, circle.Radius);
        Assert.Equal(25 * Math.PI, circle.GetArea(), 10);
        Assert.Equal(10 * Math.PI, circle.GetPerimeter(), 10);
        Assert.Equal((4d, 6d), (rectangle.Width, rectangle.Height));
        Assert.Equal(24, rectangle.GetArea());
        Assert.Equal(20, rectangle.GetPerimeter());
        Assert.Equal(3, cube.Edge);
        Assert.Equal(54, cube.GetArea());
        Assert.Equal(36, cube.GetPerimeter());
        Assert.Equal(27, ((I3DShape)cube).GetVolume());
    }

    [Theory, MemberData(nameof(InvalidSizes))]
    public void Circle_RejectsInvalidRadius(double value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(value));

    [Theory, MemberData(nameof(InvalidSizes))]
    public void Cube_RejectsInvalidEdge(double value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cube(value));

    [Theory, MemberData(nameof(InvalidSizes))]
    public void Rectangle_RejectsInvalidWidth(double value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Rectangle(value, 1));

    [Theory, MemberData(nameof(InvalidSizes))]
    public void Rectangle_RejectsInvalidHeight(double value) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Rectangle(1, value));

    [Fact]
    public void Draw_WritesDescriptionsAndPrintsShapeInfo()
    {
        using var console = new ConsoleCapture();
        var circle = new Circle(5);
        var rectangle = new Rectangle(4, 6);
        circle.Draw();
        rectangle.Draw();
        Demo.PrintShapeInfo(circle);
        string text = console.Output.ToString();
        Assert.Contains("Circle: radius=5", text);
        Assert.Contains("Rectangle: 4 x 6", text);
        Assert.Contains("Circle: area=78.54, perimeter=31.42", text);
    }

    [Fact]
    public void DrawAll_CallsEachItemAndAcceptsEmptyList()
    {
        var first = new Mock<IDrawable>(MockBehavior.Strict);
        var second = new Mock<IDrawable>(MockBehavior.Strict);
        first.Setup(x => x.Draw());
        second.Setup(x => x.Draw());
        Demo.DrawAll([]);
        Demo.DrawAll([first.Object, second.Object]);
        first.Verify(x => x.Draw(), Times.Once);
        second.Verify(x => x.Draw(), Times.Once);
        first.VerifyNoOtherCalls();
        second.VerifyNoOtherCalls();
    }
}
