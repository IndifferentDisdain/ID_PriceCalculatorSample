namespace PriceCalculator;

public class UnitTest1
{
    [Fact]
    public void SalesOrder_HappyPath()
    {
        // Arrange
        var items = new List<ISalesOrderItem>
        {
            new TaxableSalesOrderItem(sellingPrice: 19.95m, quantity: 1, productCategory: ProductCategory.Merchandise),
            new NonTaxableSalesOrderItem(sellingPrice: 4.99m, quantity: 1, productCategory: ProductCategory.RawFood)
        };

        var salesOrder = new SalesOrder();

        // Act
        foreach (var item in items)
            salesOrder.AddSalesOrderItem(item);

        // Assert
        Assert.Equal(24.94m, salesOrder.Subtotal);
        Assert.Equal(1.50m, salesOrder.SalesTax);
        Assert.Equal(26.44m, salesOrder.GrandTotal);
    }

    [Fact]
    public void SalesOrder_MultipleQuantities()
    {
        // Arrange
        var items = new List<ISalesOrderItem>
        {
            new TaxableSalesOrderItem(sellingPrice: 19.95m, quantity: 3, productCategory: ProductCategory.Merchandise),
            new NonTaxableSalesOrderItem(sellingPrice: 4.99m, quantity: 2, productCategory: ProductCategory.RawFood)
        };

        var salesOrder = new SalesOrder();

        // Act
        foreach (var item in items)
            salesOrder.AddSalesOrderItem(item);

        // Assert
        Assert.Equal(69.83m, salesOrder.Subtotal);
        Assert.Equal(4.49m, salesOrder.SalesTax);
        Assert.Equal(74.32m, salesOrder.GrandTotal);
    }
}