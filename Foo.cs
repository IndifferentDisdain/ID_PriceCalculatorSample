namespace PriceCalculator;

public interface ISalesOrderItem
{
    decimal SellingPrice { get; set; }
    public decimal Accept(ICalculator visitor);
}

public interface ITaxable
{

}

public class TaxableSalesOrderItem : ISalesOrderItem, ITaxable
{
    public decimal SellingPrice { get; set; }

    public decimal Accept(ICalculator calculator)
    {
        return calculator.Visit(this);
    }
}

public class NonTaxableSalesOrderItem : ISalesOrderItem
{
    public decimal SellingPrice { get; set; }

    public decimal Accept(ICalculator calculator)
    {
        return calculator.Visit(this);
    }
}

public interface ICalculator // visitor
{
    decimal Visit(ISalesOrderItem product);
}

public class SubtotalCalculator : ICalculator
{
    public decimal Visit(ISalesOrderItem product)
    {
        return product.SellingPrice;
    }
}

public class TaxCalculator : ICalculator
{
    private const decimal taxRate = 0.075m; // TODO: inject this
    public decimal Visit(ISalesOrderItem orderItem)
    {
        if (orderItem is not ITaxable)
            return 0.00m;

        return Math.Round(orderItem.SellingPrice * taxRate, 2);
        // return product.SellingPrice * taxRate;
    }
}