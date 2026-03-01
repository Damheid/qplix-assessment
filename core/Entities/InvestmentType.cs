namespace core.Entities;

public enum InvestmentType
{
    Fonds,
    RealEstate,
    Stock
}

internal class InvestmentTypeConverter : QPlixTypeConverter<InvestmentType> { }