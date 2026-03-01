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

        using var investmentReader = new DataReader("data/Investments.csv");
        storage.Investments = investmentReader.ReadCsv<InvestmentEntry, InvestmentEntryMap>();

        storage.Investors = storage.Investments.Where(i => i.InvestorId.StartsWith("Investor", StringComparison.OrdinalIgnoreCase))
                                               .Select(i => i.InvestorId)
                                               .Distinct();

        storage.Funds = storage.Investments.Where(i => i.InvestorId.StartsWith("Fond", StringComparison.OrdinalIgnoreCase))
                                           .Select(i => i.InvestorId)
                                           .Distinct();

        using var transactionReader = new DataReader("data/Transactions.csv");
        storage.Transactions = transactionReader.ReadCsv<TransactionEntry, TransactionEntryMap>();

        using var quotesReader = new DataReader("data/Quotes.csv");
        storage.Quotes = quotesReader.ReadCsv<QuoteEntry>();

        return storage;
    }
}
