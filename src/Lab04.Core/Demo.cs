namespace Lab02.Interfaces;

public static class Demo
{
    public static void DrawAll(List<IDrawable> shapes)
    {
        foreach (var shape in shapes) shape.Draw();
    }
    public static void PrintShapeInfo(IShape shape) =>
        Console.WriteLine($"{shape.GetType().Name}: area={shape.GetArea():F2}, " +
            $"perimeter={shape.GetPerimeter():F2}");
    public static void ProcessPayment(IPayable method, decimal amount) =>
        method.Pay(amount);
    public static void DoWork(ILogger logger)
    {
        logger.Log("Work started");
        logger.Log("Work completed");
    }
}

