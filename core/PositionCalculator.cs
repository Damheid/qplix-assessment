using core.Entities;

namespace core;

public class PositionCalculator(IQPlixStorage storage)
{
    public double Calculate(string investorId, DateOnly date)
    {
        double stockPosition = 0;
        double realEstatePositon = 0;
        double fundsPosition = 0;

        var investimentsOfInvestor = storage.Investments.Where(i => i.InvestorId == investorId);

        // Stocks

        var stocksOfInvestor = investimentsOfInvestor.Where(i => i.InvestmentType == InvestmentType.Stock);

        stockPosition = CalculateStocks(stocksOfInvestor, date);

        // Real Estate

        var realEstateOfInvestor = investimentsOfInvestor.Where(i => i.InvestmentType == InvestmentType.RealEstate);
        realEstatePositon = CalculateRealState(realEstateOfInvestor, date);

        // Funds

        var fundsOfInvestor = investimentsOfInvestor.Where(i => i.InvestmentType == InvestmentType.Fonds);
        fundsPosition = CalculateFunds(fundsOfInvestor, date);

        return stockPosition + realEstatePositon + fundsPosition;
    }

    private double CalculateStocks(IEnumerable<InvestmentEntry> stocks, DateOnly date)
    {
        double total = 0;
        foreach (var stock in stocks)
        {
            var quote = LoadLatestQuote(storage.Quotes, stock.ISIN, date);
            var totalStocks = SumTransactions(storage.Transactions, stock.InvestmentId, date);
            total += totalStocks * quote;
        }

        return total;
    }

    private double CalculateRealState(IEnumerable<InvestmentEntry> realEstate, DateOnly date)
    {
        double total = 0;
        foreach (var rs in realEstate)
        {
            total += SumTransactions(storage.Transactions, rs.InvestmentId, date);
        }

        return total;
    }

    private double CalculateFunds(IEnumerable<InvestmentEntry> funds, DateOnly date)
    {
        double total = 0;
        foreach (var fund in funds)
        {
            var percentage = SumTransactions(storage.Transactions, fund.InvestmentId, date);
            var fundValue = Calculate(fund.FondsInvestor, date);

            total = fundValue * (percentage / 100);

        }
        return total;
    }

    private static double LoadLatestQuote(IEnumerable<QuoteEntry> quotes, string isin, DateOnly date)
    {
        return quotes.Where(q => q.ISIN == isin && q.Date <= date)
                     .OrderByDescending(q => q.Date)
                     .Select(q => q.PricePerShare)
                     .First();
    }

    private static double SumTransactions(IEnumerable<TransactionEntry> transactions, string investmentId, DateOnly date)
    {
        return transactions.Where(t => t.InvestmentId == investmentId && t.Date <= date)
                           .Sum(t => t.Value);
    }
}
