using CsvHelper.Configuration;

namespace core.Entities;

public class TransactionEntry
{
    public string InvestmentId { get; set; } = default!;
    public TransactionType Type { get; set; }
    public DateOnly Date { get; set; }
    public double Value { get; set; }
}

internal class TransactionEntryMap : ClassMap<TransactionEntry>
{
    public TransactionEntryMap()
    {
        Map(m => m.Type).TypeConverter<TransactionTypeConverter>();
    }
}