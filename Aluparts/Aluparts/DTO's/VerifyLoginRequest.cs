namespace Aluparts.API.DTO_s
{
    public class VerifyLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
