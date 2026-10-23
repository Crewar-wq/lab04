using Lab02.Interfaces;
using Moq;
using Xunit;

namespace Lab04.Tests;

public sealed class DevicesAndPaymentsTests
{
    [Fact]
    public void Devices_ExposeExpectedOperations()
    {
        using var console = new ConsoleCapture();
        new Printer().Print("Report");
        Assert.Equal("Scanned document", new Scanner().Scan());
        var mfd = new MultifunctionDevice();
        mfd.Print("Copy");
        Assert.Equal("MFD scanned document", mfd.Scan());
        mfd.SendFax("Report", "12345");
        Assert.Equal("Printer: Report\nMFD print: Copy\n" +
            "MFD fax to 12345: Report\n", console.Output.ToString().Replace("\r", ""));
    }

    [Fact]
    public void LegacyDevices_KeepTheirOriginalEmptyOperations()
    {
        using var console = new ConsoleCapture();
        Lab02.Interfaces.Legacy.IDevice[] devices =
            [new Lab02.Interfaces.Legacy.Printer(), new Lab02.Interfaces.Legacy.Scanner()];
        foreach (var device in devices)
        {
            device.Print(); device.Scan(); device.Fax();
        }
        Assert.Equal("", console.Output.ToString());
    }

    [Theory, InlineData(true), InlineData(false)]
    public void Payment_PositiveAmountWritesExpectedMessage(bool card)
    {
        using var console = new ConsoleCapture();
        IPayable payment = card ? new CreditCard() : new Cash();
        payment.Pay(12.50m);
        Assert.Contains(card ? "Credit card payment: 12.50" :
            "Cash payment: 12.50", console.Output.ToString());
    }

    [Theory]
    [InlineData(true, 0), InlineData(true, -1)]
    [InlineData(false, 0), InlineData(false, -1)]
    public void Payment_RejectsNonPositiveAmount(bool card, int amount)
    {
        using var console = new ConsoleCapture();
        IPayable payment = card ? new CreditCard() : new Cash();
        Assert.Throws<ArgumentOutOfRangeException>(() => payment.Pay(amount));
        Assert.Equal("", console.Output.ToString());
    }

    [Fact]
    public void ProcessPayment_DelegatesExactAmount()
    {
        var payment = new Mock<IPayable>(MockBehavior.Strict);
        payment.Setup(x => x.Pay(1500m));
        Demo.ProcessPayment(payment.Object, 1500m);
        payment.Verify(x => x.Pay(1500m), Times.Once);
        payment.VerifyNoOtherCalls();
    }

    [Fact]
    public void ProcessPayment_PropagatesDependencyFailure()
    {
        var payment = new Mock<IPayable>();
        var failure = new InvalidOperationException("Declined");
        payment.Setup(x => x.Pay(1m)).Throws(failure);
        Assert.Same(failure, Assert.Throws<InvalidOperationException>(
            () => Demo.ProcessPayment(payment.Object, 1m)));
    }
}
