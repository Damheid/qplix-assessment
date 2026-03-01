using core.Entities;

namespace core;

public interface IQPlixStorage
{
    IEnumerable<string> Investors { get; set; }
    IEnumerable<string> Funds { get; set; }
    IEnumerable<InvestmentEntry> Investments { get; set; }
    IEnumerable<TransactionEntry> Transactions { get; set; }
    IEnumerable<QuoteEntry> Quotes { get; set; }
}

internal class InMemoryStorage : IQPlixStorage
{
    public IEnumerable<string> Investors { get; set; } = default!;
    public IEnumerable<string> Funds { get; set; } = default!;
    public IEnumerable<InvestmentEntry> Investments { get; set; } = default!;
    public IEnumerable<TransactionEntry> Transactions { get; set; } = default!;
    public IEnumerable<QuoteEntry> Quotes { get; set; } = default!;
}