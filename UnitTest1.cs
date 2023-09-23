namespace PriceCalculator;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var products = new List<ISalesOrderItem>
        {
            new TaxableSalesOrderItem { SellingPrice = 19.95m } ,
            new NonTaxableSalesOrderItem { SellingPrice = 4.99m }
        };

        var subtotalCalculator = new SubtotalCalculator();
        var taxCalculator = new TaxCalculator();

        // Act
        decimal subtotal = 0.00m;
        decimal tax = 0.00m;

        foreach (var product in products)
        {
            subtotal += product.Accept(subtotalCalculator);
            tax += product.Accept(taxCalculator);
        }

        var grandTotal = subtotal + tax;

        // Assert
        Assert.Equal(24.94m, subtotal);
        Assert.Equal(1.50m, tax);
        Assert.Equal(26.44m, grandTotal);

    }
}