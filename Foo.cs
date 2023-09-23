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
}

public interface ITaxable { }

public abstract class SalesOrderItemBase : ISalesOrderItem
{
    public SalesOrderItemBase(decimal sellingPrice, int quantity, ProductCategory productCategory)
    {
        SellingPrice = sellingPrice;
        Quantity = quantity;
        ProductCategory = productCategory;
    }
    public decimal SellingPrice { get; set; }
    public ProductCategory ProductCategory { get; set; }
    public int Quantity { get; set; }
}

public class TaxableSalesOrderItem : SalesOrderItemBase, ITaxable
{
    public TaxableSalesOrderItem(decimal sellingPrice, int quantity, ProductCategory productCategory)
        : base(sellingPrice, quantity, productCategory) { }
}

public class NonTaxableSalesOrderItem : SalesOrderItemBase
{
    public NonTaxableSalesOrderItem(decimal sellingPrice, int quantity, ProductCategory productCategory)
        : base(sellingPrice, quantity, productCategory) { }
}

public class SalesOrder
{
    private readonly IList<ISalesOrderItem> _items = new List<ISalesOrderItem>();

    private readonly ICalculator _subtotalCalculator = new SubtotalCalculator();

    private readonly ICalculator _salesTaxCalculator = new SalesTaxCalculator();

    private readonly ICalculator _discountCalculator = new DiscountCalculator();

    public ReadOnlyCollection<ISalesOrderItem> Items => _items.AsReadOnly();

    public decimal Subtotal { get; private set; }

    public decimal Discount { get; private set; }

    public decimal SalesTax { get; private set; }

    public decimal GrandTotal => Subtotal - Discount + SalesTax;

    public void AddSalesOrderItem(ISalesOrderItem item)
    {
        if (item is null)
            return;

        _items.Add(item);
        Subtotal = _subtotalCalculator.Calculate(_items);
        Discount = _discountCalculator.Calculate(_items);
        SalesTax = _salesTaxCalculator.Calculate(_items);
    }
}

public interface ICalculator // visitor
{
    decimal Calculate(IEnumerable<ISalesOrderItem> items);
}

public class SubtotalCalculator : ICalculator
{
    public decimal Calculate(IEnumerable<ISalesOrderItem> items)
    {
        return items.Sum(x => x.SellingPrice * x.Quantity);
    }
}

public class SalesTaxCalculator : ICalculator
{
    private const decimal taxRate = 0.075m; // TODO: inject this
    public decimal Calculate(IEnumerable<ISalesOrderItem> items)
    {
        return Math.Round(items.Where(x => x is ITaxable).Sum(y => y.SellingPrice * taxRate * y.Quantity), 2);
    }
}

/// <summary>
/// Adds a 10% discount if subtotal is greater than or equal to $99.00
/// </summary>
public class DiscountCalculator : ICalculator
{
    public decimal Calculate(IEnumerable<ISalesOrderItem> items)
    {
        var discountThreshold = 99m;
        var subtotalCalculator = new SubtotalCalculator();
        var subtotal = subtotalCalculator.Calculate(items);
        return subtotal >= discountThreshold ? Math.Round(subtotal * .1m, 2) : 0;
    }
}