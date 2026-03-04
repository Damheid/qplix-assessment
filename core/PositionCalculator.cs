using core.Buffer;
using core.Entities;
using ErrorOr;

namespace core;

public class PositionCalculator(IQPlixStorage storage)
{
    private readonly QPlixBuffer FundBuffer = [];

    public ErrorOr<double> Calculate(string investorId, DateOnly date)
    {
        if (storage.Investments.Contains(investorId) == false)
        {
            Error.NotFound(description: $"There is no investor with Id: {investorId}");
        }

        var investimentsOfInvestor = storage.Investments[investorId].ToArray();

        var filteredTransactions = investimentsOfInvestor.SelectMany(i => storage.Transactions[i.InvestmentId])
                                                         .Where(t => t.Date <= date)
                                                         .ToArray();

        return Calculate(date, filteredTransactions, investimentsOfInvestor);
    }

    private double Calculate(DateOnly date, IEnumerable<TransactionEntry> transactions, IEnumerable<InvestmentEntry> investments)
    {
        double stockPosition = 0;
        double realEstatePositon = 0;
        double fundsPosition = 0;

        // Stocks

        var stocks = investments.Where(i => i.InvestmentType == InvestmentType.Stock)
                                .ToArray();

        stockPosition = CalculateStocks(stocks, date, transactions);

        // Real Estate

        var realEstate = investments.Where(i => i.InvestmentType == InvestmentType.RealEstate)
                                    .ToArray();

        realEstatePositon = CalculateRealState(realEstate, transactions);

        // Funds

        var funds = investments.Where(i => i.InvestmentType == InvestmentType.Fonds)
                               .ToArray();

        fundsPosition = CalculateFunds(funds, date, transactions);

        return stockPosition + realEstatePositon + fundsPosition;
    }

    private double CalculateStocks(IEnumerable<InvestmentEntry> stocks, DateOnly date, IEnumerable<TransactionEntry> transactions)
    {
        double total = 0;

        var distinctIsins = stocks.Select(s => s.ISIN!)
                                  .Distinct()
                                  .ToArray();


        var filteredQuotes = stocks.SelectMany(s => storage.Quotes[s.ISIN])// storage.Quotes.Where(q => distinctIsins.Contains(q.ISIN))
                                           .ToArray();

        var latestQuotes = filteredQuotes.Where(q => q.Date <= date)
                                         .GroupBy(q => q.ISIN)
                                         .Select(g => g.MaxBy(q => q.Date))
                                         .ToDictionary(k => k.ISIN, v => v.PricePerShare);

        total = stocks.Join(latestQuotes, a => a.ISIN, b => b.Key, (a, b) => new { a.InvestmentId, b.Value })
                      .Join(transactions, a => a.InvestmentId, b => b.InvestmentId, (a, b) => new { quote = a.Value, stocksCount = b.Value })
                      .Sum(j => j.quote * j.stocksCount);

        return total;
    }

    private double CalculateRealState(IEnumerable<InvestmentEntry> realEstate, IEnumerable<TransactionEntry> transactions)
    {
        double total = realEstate.Join(transactions, a => a.InvestmentId, b => b.InvestmentId, (a, b) => b.Value)
                                 .Sum();

        return total;
    }

    private double CalculateFunds(IEnumerable<InvestmentEntry> funds, DateOnly date, IEnumerable<TransactionEntry> transactions)
    {
        double total = 0;

        var distinctFunds = funds.Select(f => f.FondsInvestor)
                                 .Distinct()
                                 .ToArray();

        foreach (var fundAsInvestor in distinctFunds)
        {
            var key = new BufferKey() { Name = fundAsInvestor, Date = date };
            if (FundBuffer.ContainsKey(key))
                continue;

            var result = Calculate(fundAsInvestor, date);

            FundBuffer.Add(key, result.Value); // Assume here there are no errors in the dataset.
        }

        foreach (var fund in funds)
        {
            var key = new BufferKey() { Name = fund.FondsInvestor, Date = date };
            var percentage = SumTransactions(transactions, fund.InvestmentId, date);
            var fundValue = FundBuffer[key];

            total += fundValue * (percentage / 100);
        }
        return total;
    }

    private static double SumTransactions(IEnumerable<TransactionEntry> transactions, string investmentId, DateOnly date)
    {
        return transactions.Where(t => t.InvestmentId == investmentId && t.Date <= date)
                           .Sum(t => t.Value);
    }
}
