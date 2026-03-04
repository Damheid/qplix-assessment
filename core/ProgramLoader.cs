using System.Collections.ObjectModel;
using core.Entities;

namespace core;

public class ProgramLoader
{
    public static PositionCalculator Load()
    {
        var storage = LoadData();
        return new PositionCalculator(storage);
    }

    private static InMemoryStorage LoadData()
    {
        InMemoryStorage storage = new();

        // Investments
        using var investmentReader = new DataReader("/home/heiddam/Repos/qPlix/data/Investments.csv");
        var investments = investmentReader.ReadCsv<InvestmentEntry, InvestmentEntryMap>();

        storage.Investors = investments.Where(i => i.InvestorId.StartsWith("Investor", StringComparison.OrdinalIgnoreCase))
                                               .Select(i => i.InvestorId)
                                               .Distinct()
                                               .ToArray();

        storage.Funds = investments.Where(i => i.InvestorId.StartsWith("Fond", StringComparison.OrdinalIgnoreCase))
                                           .Select(i => i.InvestorId)
                                           .Distinct()
                                           .ToArray();

        storage.Investments = investments.ToLookup(i => i.InvestorId);

        // Quotes
        using var quotesReader = new DataReader("/home/heiddam/Repos/qPlix/data/Quotes.csv");
        var quotes = quotesReader.ReadCsv<QuoteEntry>();

        storage.Quotes = quotes.ToLookup(q => q.ISIN);

        // Transactions

        using var transactionReader = new DataReader("/home/heiddam/Repos/qPlix/data/Transactions.csv");
        var transactions = transactionReader.ReadCsv<TransactionEntry, TransactionEntryMap>();

        storage.Transactions = transactions.ToLookup(t => t.InvestmentId);

        return storage;
    }
}
