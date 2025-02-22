namespace server.ApplicationCore.DomModels;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CategoryLimit> CategoryLimits { get; set; } = new List<CategoryLimit>();

    public virtual ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();

    public virtual User User { get; set; } = null!;
}
