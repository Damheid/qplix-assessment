namespace core.Entities;

public enum TransactionType
{
    Building,
    Estate,
    Percentage,
    Shares
}

internal class TransactionTypeConverter : QPlixTypeConverter<TransactionType> { }
