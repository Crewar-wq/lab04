using Lab02.Interfaces;

namespace Lab04.App;

public static class DemoApplication
{
    public static void Run(string logPath, ILogStorage storage)
    {
        Console.WriteLine("=== Task 1: basic interfaces ===");
        IMovable movable = new Point(1, 2);
        movable.Move(10, 20);
        Console.WriteLine(movable);
        var circle = new Circle(5);
        var rectangle = new Rectangle(4, 6);
        Demo.DrawAll(new List<IDrawable> { circle, rectangle });
        
        Console.WriteLine("=== Task 2: interface inheritance ===");
        Demo.PrintShapeInfo(circle);
        Demo.PrintShapeInfo(rectangle);
        I3DShape cube = new Cube(3);
        Demo.PrintShapeInfo(cube);
        Console.WriteLine($"Cube volume={cube.GetVolume():F2}");
        
        Console.WriteLine("=== Task 3: interface segregation ===");
        Lab02.Interfaces.Legacy.IDevice legacyPrinter =
            new Lab02.Interfaces.Legacy.Printer();
        legacyPrinter.Scan(); // Empty method exposes the ISP violation.
        Console.WriteLine("Legacy Printer.Scan: empty unsupported operation");
        IPrinter printer = new Printer();
        printer.Print("Laboratory report");
        IScanner scanner = new Scanner();
        Console.WriteLine(scanner.Scan());
        var mfd = new MultifunctionDevice();
        ((IPrinter)mfd).Print("Copy");
        Console.WriteLine(((IScanner)mfd).Scan());
        ((IFax)mfd).SendFax("Report", "12345");
        
        Console.WriteLine("=== Task 4: polymorphism ===");
        Demo.ProcessPayment(new CreditCard(), 1500m);
        Demo.ProcessPayment(new Cash(), 250m);
        Demo.DoWork(new ConsoleLogger());
        Demo.DoWork(new FileLogger(logPath, storage));
        Console.WriteLine("File log written");
        
    }
}
