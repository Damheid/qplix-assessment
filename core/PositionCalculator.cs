using core.Buffer;
using core.Entities;

namespace core;

public class PositionCalculator(IQPlixStorage storage)
{
    private readonly QPlixBuffer FundBuffer = [];
    private readonly QPlixBuffer QuotesBuffer = [];

    public double Calculate(string investorId, DateOnly date)
    {
        double stockPosition = 0;
        double realEstatePositon = 0;
        double fundsPosition = 0;

        var investimentsOfInvestor = storage.Investments.Where(i => i.InvestorId == investorId)
                                                        .ToArray();

        // Stocks

        var stocksOfInvestor = investimentsOfInvestor.Where(i => i.InvestmentType == InvestmentType.Stock)
                                                     .ToArray();

        stockPosition = CalculateStocks(stocksOfInvestor, date);

        // Real Estate

        var realEstateOfInvestor = investimentsOfInvestor.Where(i => i.InvestmentType == InvestmentType.RealEstate)
                                                         .ToArray();
        realEstatePositon = CalculateRealState(realEstateOfInvestor, date);

        // Funds

        var fundsOfInvestor = investimentsOfInvestor.Where(i => i.InvestmentType == InvestmentType.Fonds)
                                                    .ToArray();
        fundsPosition = CalculateFunds(fundsOfInvestor, date);

        return stockPosition + realEstatePositon + fundsPosition;
    }

    private double CalculateStocks(IEnumerable<InvestmentEntry> stocks, DateOnly date)
    {
        double total = 0;

        var distinctIsins = stocks.Select(s => s.ISIN)
                                  .Distinct()
                                  .ToArray();

        var filteredIsins = storage.Quotes.Where(q => distinctIsins.Contains(q.ISIN))
                                        .ToArray();


        var filteredTransactions = FilterTransactions(stocks);

        foreach (var stock in stocks)
        {
            var quote = LoadLatestQuote(filteredIsins, stock.ISIN, date);
            var totalStocks = SumTransactions(filteredTransactions, stock.InvestmentId, date);
            total += totalStocks * quote;
        }

        return total;
    }

    private double CalculateRealState(IEnumerable<InvestmentEntry> realEstate, DateOnly date)
    {
        double total = 0;

        var filteredTransactions = FilterTransactions(realEstate);

        foreach (var rs in realEstate)
        {
            total += SumTransactions(filteredTransactions, rs.InvestmentId, date);
        }

        return total;
    }

    private double CalculateFunds(IEnumerable<InvestmentEntry> funds, DateOnly date)
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

            var fundValue = Calculate(fundAsInvestor, date);
            FundBuffer.Add(key, fundValue);
        }

        var filteredTransactions = FilterTransactions(funds);

        foreach (var fund in funds)
        {
            var key = new BufferKey() { Name = fund.FondsInvestor, Date = date };
            var percentage = SumTransactions(filteredTransactions, fund.InvestmentId, date);
            var fundValue = FundBuffer[key];

            total += fundValue * (percentage / 100);
        }
        return total;
    }

    private TransactionEntry[] FilterTransactions(IEnumerable<InvestmentEntry> investments)
    {
        var distinctInvestments = investments.Select(s => s.InvestmentId)
                                             .Distinct()
                                             .ToArray();

        var filteredTransactions = storage.Transactions.Where(t => distinctInvestments.Contains(t.InvestmentId))
                                                       .ToArray();

        return filteredTransactions;
    }

    private double LoadLatestQuote(IEnumerable<QuoteEntry> quotes, string isin, DateOnly date)
    {
        var key = new BufferKey { Name = isin, Date = date };

        if (QuotesBuffer.TryGetValue(key, out double value))
            return value;

        var quoteValue = quotes.Where(q => q.ISIN == isin && q.Date <= date)
                                .OrderByDescending(q => q.Date)
                                .Select(q => q.PricePerShare)
                                .First();

        QuotesBuffer.Add(key, quoteValue);

        return quoteValue;
    }

    private static double SumTransactions(IEnumerable<TransactionEntry> transactions, string investmentId, DateOnly date)
    {
        return transactions.Where(t => t.InvestmentId == investmentId && t.Date <= date)
                           .Sum(t => t.Value);
    }
}
