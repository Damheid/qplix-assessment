namespace core.Entities;

public class QuoteEntry
{
    public string ISIN { get; set; } = default!;
    public DateOnly Date { get; set; }
    public double PricePerShare { get; set; }
}
