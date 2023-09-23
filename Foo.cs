using System.Collections.ObjectModel;

namespace PriceCalculator;

public enum ProductCategory
{
    NotSet,
    RawFood,
    Merchandise
}

public interface ISalesOrderItem
{
    decimal SellingPrice { get; set; }
    ProductCategory ProductCategory { get; set; }
    int Quantity { get; set; }
    public decimal Accept(ICalculator visitor);
}

public interface ITaxable { }

public class TaxableSalesOrderItem : ISalesOrderItem, ITaxable
{
    // TODO: consider a base class for SalesOrderItem?
    public TaxableSalesOrderItem(decimal sellingPrice, int quantity, ProductCategory productCategory)
    {
        // TODO: validate quantity and sellingPrice > 0
        SellingPrice = sellingPrice;
        Quantity = quantity;
        ProductCategory = productCategory;
    }

    public decimal SellingPrice { get; set; }

    public ProductCategory ProductCategory { get; set; }

    public int Quantity { get; set; } = 1;

    public decimal Accept(ICalculator calculator)
    {
        return calculator.Visit(this);
    }
}

public class NonTaxableSalesOrderItem : ISalesOrderItem
{
    public NonTaxableSalesOrderItem(decimal sellingPrice, int quantity, ProductCategory productCategory)
    {
        // TODO: validate quantity and sellingPrice > 0
        SellingPrice = sellingPrice;
        Quantity = quantity;
        ProductCategory = productCategory;
    }

    public decimal SellingPrice { get; set; }

    public ProductCategory ProductCategory { get; set; }

    public int Quantity { get; set; } = 1;

    public decimal Accept(ICalculator calculator)
    {
        return calculator.Visit(this);
    }
}

public class SalesOrder
{
    private readonly IList<ISalesOrderItem> _items = new List<ISalesOrderItem>();

    private readonly ICalculator _subtotalCalculator = new SubtotalCalculator();

    private readonly ICalculator _salesTaxCalculator = new SalesTaxCalculator();

    public ReadOnlyCollection<ISalesOrderItem> Items => _items.AsReadOnly();

    public decimal Subtotal { get; private set; }

    public decimal SalesTax { get; private set; }

    public decimal GrandTotal => Subtotal + SalesTax;

    public void AddSalesOrderItem(ISalesOrderItem item)
    {
        if (item is null)
            return;

        _items.Add(item);
        Subtotal += item.Accept(_subtotalCalculator);
        SalesTax += item.Accept(_salesTaxCalculator);
    }
}

public interface ICalculator // visitor
{
    decimal Visit(ISalesOrderItem product);
}

public class SubtotalCalculator : ICalculator
{
    public decimal Visit(ISalesOrderItem orderItem)
    {
        return orderItem.SellingPrice * orderItem.Quantity;
    }
}

public class SalesTaxCalculator : ICalculator
{
    private const decimal taxRate = 0.075m; // TODO: inject this
    public decimal Visit(ISalesOrderItem orderItem)
    {
        if (orderItem is not ITaxable)
            return 0.00m;

        return Math.Round(orderItem.SellingPrice * taxRate * orderItem.Quantity, 2);
    }
}