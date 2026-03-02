using CsvHelper.Configuration;

namespace core.Entities;

public class InvestmentEntry
{
    public string InvestorId { get; set; } = default!;
    public string InvestmentId { get; set; } = default!;
    public InvestmentType InvestmentType { get; set; }
    public string? ISIN { get; set; }
    public string? City { get; set; }
    public string? FondsInvestor { get; set; }
}


internal class InvestmentEntryMap : ClassMap<InvestmentEntry>
{
    public InvestmentEntryMap()
    {
        Map(m => m.InvestorId);
        Map(m => m.InvestmentId);
        Map(m => m.InvestmentType).TypeConverter<InvestmentTypeConverter>();
        Map(m => m.ISIN);
        Map(m => m.City);
        Map(m => m.FondsInvestor);
    }
}