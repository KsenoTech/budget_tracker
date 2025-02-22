namespace server.ApplicationCore.DomModels;

public partial class IncomeItem
{
    public int Id { get; set; }

    public int IncomeCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public virtual IncomeCategory IncomeCategory { get; set; } = null!;
}
