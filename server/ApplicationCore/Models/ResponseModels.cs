using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Models
{
    public class ResponseModels
    {
        public class RegisterDto
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class AuthResponseDto
        {
            public string Token { get; set; }
        }


        #region ExpenseCategory
            public class CreateExpenseCategoryDto
            {
                public string Name { get; set; } = null!;
                public string UserId { get; set; } = null!;
                public DateTime CreatedAt { get; set; }
                public List<CategoryLimitDto> CategoryLimits { get; set; } = new List<CategoryLimitDto>();
                public List<ExpenseItemDto> ExpenseItems { get; set; } = new List<ExpenseItemDto>();
            }
            public class CategoryLimitDto
            {
                public decimal LimitAmount { get; set; }
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
            }
            public class ExpenseItemDto
            {
                public string Name { get; set; } = null!;
                public decimal Amount { get; set; }
                public DateTime TransactionDate { get; set; }
            }
        #endregion


        #region
            public class CreateExpenseItem
            {
                public string Name { get; set; } = null!;
                public decimal Amount { get; set; }
                public DateTime TransactionDate { get; set; }
                public string CategoryName { get; set; } = null!; // Название категории вместо ID
            }


        #endregion

    }
}
