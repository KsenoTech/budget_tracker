using Microsoft.AspNetCore.Identity;

namespace server.ApplicationCore.DomModels;

public partial class User : IdentityUser
{  
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();

    public virtual ICollection<IncomeCategory> IncomeCategories { get; set; } = new List<IncomeCategory>();
}
