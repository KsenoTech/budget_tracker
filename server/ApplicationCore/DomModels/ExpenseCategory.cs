namespace server.ApplicationCore.DomModels;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CategoryLimit> CategoryLimits { get; set; } = new List<CategoryLimit>();

    public virtual ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();

    public virtual Client User { get; set; } = null!;
}
