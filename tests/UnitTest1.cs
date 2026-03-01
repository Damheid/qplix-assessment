using core;
using core.Entities;
using Shouldly;

namespace tests;

public class TestStorage : IQPlixStorage
{
    public IEnumerable<string> Investors { get; set; } = [];
    public IEnumerable<string> Funds { get; set; } = [];
    public IEnumerable<InvestmentEntry> Investments { get; set; } = [];
    public IEnumerable<TransactionEntry> Transactions { get; set; } = [];
    public IEnumerable<QuoteEntry> Quotes { get; set; } = [];
}

public class BasicTests
{
    [Fact]
    public void Should_Calculate_Stock_Value()
    {
        var storage = new TestStorage
        {
            Investors = ["InvestorA"],
            Investments = [
                new InvestmentEntry {
                    InvestorId = "InvestorA",
                    InvestmentId = "InvestmentA",
                    InvestmentType = InvestmentType.Stock,
                    ISIN = "ISINA"
                }
            ],
            Transactions = [
                new TransactionEntry {
                    InvestmentId = "InvestmentA",
                    Date = new DateOnly(2026, 01, 01),
                    Type = TransactionType.Shares,
                    Value = 2
                },
                new TransactionEntry {
                    InvestmentId = "InvestmentA",
                    Date = new DateOnly(2026, 01, 15),
                    Type = TransactionType.Shares,
                    Value = 2
                }
            ],
            Quotes = [
                new QuoteEntry {
                    ISIN = "ISINA",
                    Date = new DateOnly(2026, 01, 01),
                    PricePerShare = 5
                },
                new QuoteEntry {
                    ISIN = "ISINA",
                    Date = new DateOnly(2026, 02, 01),
                    PricePerShare = 8
                }
            ]
        };

        var calculator = new PositionCalculator(storage);

        calculator.Calculate("InvestorA", new DateOnly(2026, 01, 01)).ShouldBe(10);
        calculator.Calculate("InvestorA", new DateOnly(2026, 01, 20)).ShouldBe(20);
        calculator.Calculate("InvestorA", new DateOnly(2026, 02, 01)).ShouldBe(32);
    }

    [Fact]
    public void Should_Calculate_RealEstate_Value()
    {
        var storage = new TestStorage
        {
            Investors = ["InvestorA"],
            Investments = [
                new InvestmentEntry {
                    InvestorId = "InvestorA",
                    InvestmentId = "InvestmentA",
                    InvestmentType = InvestmentType.RealEstate
                }
            ],
            Transactions = [
                new TransactionEntry {
                    InvestmentId = "InvestmentA",
                    Date = new DateOnly(2026, 01, 01),
                    Type = TransactionType.Estate,
                    Value = 5000
                },
                new TransactionEntry {
                    InvestmentId = "InvestmentA",
                    Date = new DateOnly(2026, 01, 01),
                    Type = TransactionType.Building,
                    Value = 100
                }
            ]
        };

        var calculator = new PositionCalculator(storage);

        calculator.Calculate("InvestorA", new DateOnly(2026, 01, 01)).ShouldBe(5100);
    }

    [Fact]
    public void Should_Calculate_Fund_Value()
    {
        var storage = new TestStorage
        {
            Investors = ["InvestorA"],
            Investments = [
                new InvestmentEntry {
                    InvestorId = "InvestorA",
                    InvestmentId = "InvestmentA",
                    InvestmentType = InvestmentType.Fonds,
                    FondsInvestor = "FundA"
                },
                new InvestmentEntry {
                    InvestorId = "FundA",
                    InvestmentId = "InvestmentB",
                    InvestmentType = InvestmentType.Stock,
                    ISIN = "ISINA"
                }
            ],
            Transactions = [
                new TransactionEntry {
                    InvestmentId = "InvestmentA",
                    Date = new DateOnly(2026, 01, 01),
                    Type = TransactionType.Shares,
                    Value = 50
                },
                new TransactionEntry {
                    InvestmentId = "InvestmentB",
                    Date = new DateOnly(2026, 01, 01),
                    Type = TransactionType.Shares,
                    Value = 2
                },
                new TransactionEntry {
                    InvestmentId = "InvestmentB",
                    Date = new DateOnly(2026, 01, 15),
                    Type = TransactionType.Shares,
                    Value = 2
                }
            ],
            Quotes = [
                new QuoteEntry {
                    ISIN = "ISINA",
                    Date = new DateOnly(2026, 01, 01),
                    PricePerShare = 5
                },
                new QuoteEntry {
                    ISIN = "ISINA",
                    Date = new DateOnly(2026, 02, 01),
                    PricePerShare = 8
                }
            ]
        };

        var calculator = new PositionCalculator(storage);

        calculator.Calculate("InvestorA", new DateOnly(2026, 01, 01)).ShouldBe(5);
        calculator.Calculate("InvestorA", new DateOnly(2026, 01, 20)).ShouldBe(10);
        calculator.Calculate("InvestorA", new DateOnly(2026, 02, 01)).ShouldBe(16);
    }
}
