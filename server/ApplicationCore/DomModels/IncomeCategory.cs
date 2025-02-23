namespace server.ApplicationCore.DomModels;

public partial class IncomeCategory
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<IncomeItem> IncomeItems { get; set; } = new List<IncomeItem>();

    public virtual Client User { get; set; } = null!;
}
