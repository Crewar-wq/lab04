namespace Lab02.Interfaces;

public interface IPayable { void Pay(decimal amount); }
public sealed class CreditCard : IPayable
{
    public void Pay(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Console.WriteLine($"Credit card payment: {amount:F2}");
    }
}
public sealed class Cash : IPayable
{
    public void Pay(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Console.WriteLine($"Cash payment: {amount:F2}");
    }
}

