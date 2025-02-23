namespace server.ApplicationCore.DomModels
{

    public partial class CategoryLimit
    {
        public int Id { get; set; }
        public int ExpenseCategoryId { get; set; }
        public decimal LimitAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;
    }
}
