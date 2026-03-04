using core.Entities;

namespace core;

public interface IQPlixStorage
{
    IEnumerable<string> Investors { get; set; }
    IEnumerable<string> Funds { get; set; }
    ILookup<string, InvestmentEntry> Investments { get; set; }
    ILookup<string, TransactionEntry> Transactions { get; set; }
    ILookup<string, QuoteEntry> Quotes { get; set; }
}

internal class InMemoryStorage : IQPlixStorage
{
    public IEnumerable<string> Investors { get; set; } = default!;
    public IEnumerable<string> Funds { get; set; } = default!;
    public ILookup<string, InvestmentEntry> Investments { get; set; } = default!;
    public ILookup<string, TransactionEntry> Transactions { get; set; } = default!;
    public ILookup<string, QuoteEntry> Quotes { get; set; } = default!;
}