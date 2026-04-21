namespace Aluparts.API.DTO_s
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public bool RequiresMfa { get; set; }
        public string? Token { get; set; }
    }
}
