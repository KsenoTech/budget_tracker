namespace server.ApplicationCore.DomModels;

public partial class ExpenseItem
{
    public int Id { get; set; }

    public int ExpenseCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }

    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;
}
