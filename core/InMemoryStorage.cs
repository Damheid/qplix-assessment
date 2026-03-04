using core.Entities;

namespace core;

public interface IQPlixStorage
{
    ILookup<string, InvestmentEntry> Investments { get; set; }
    ILookup<string, TransactionEntry> Transactions { get; set; }
    ILookup<string, QuoteEntry> Quotes { get; set; }
}

internal class InMemoryStorage : IQPlixStorage
{
    public ILookup<string, InvestmentEntry> Investments { get; set; } = default!;
    public ILookup<string, TransactionEntry> Transactions { get; set; } = default!;
    public ILookup<string, QuoteEntry> Quotes { get; set; } = default!;
}