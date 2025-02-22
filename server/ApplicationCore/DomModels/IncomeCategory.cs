namespace server.ApplicationCore.DomModels;

public partial class IncomeCategory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<IncomeItem> IncomeItems { get; set; } = new List<IncomeItem>();

    public virtual User User { get; set; } = null!;
}
