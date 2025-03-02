using server.ApplicationCore.DomModels;

namespace server.ApplicationCore.Models
{
    public class ResponseModels
    {
        #region AUTH
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
        #endregion


        #region EXPENSE
            public class CategoryLimitDTO
            {
                public int CategoryId { get; set; }
                public decimal LimitAmount { get; set; }
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
            }
        #endregion


        #region INCOME

        #endregion


        #region UNION
        public class CreateCategory
            {
                public string Name { get; set; } = null!;
                public string UserId { get; set; } = null!;
                public DateTime CreatedAt { get; set; }
            }
            public class CreateItem
            {
                public string Name { get; set; } = null!;
                public decimal Amount { get; set; }
                public DateTime TransactionDate { get; set; }
                public string CategoryName { get; set; } = null!;
            }

            public class UpdateCategory
            {
                public string Name { get; set; } = null!;
            }
            
            public class UpdateItem
            {
                public string Name { get; set; } = null!;
                public decimal Amount { get; set; }
            }
        #endregion
    }
}
