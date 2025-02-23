namespace server.ApplicationCore.Models
{
    public class ResponseModels
    {
        public class RegisterDto
        {
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class AuthResponseDto
        {
            public string Token { get; set; }
        }
    }
}
