namespace server.ApplicationCore.Models
{
    public class ResponseModels
    {
        public class RegisterDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class LoginDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class AuthResponseDto
        {
            public string Token { get; set; }
        }
    }
}
