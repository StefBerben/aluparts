using Aluparts.API.DTO_s;
using Aluparts.DataLayer.Entities;
using Aluparts.DataLayer.Interfaces;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using static Aluparts.DataLayer.Entities.User;

namespace Aluparts.BusinessLayer;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(string email, string password, UserRole role)
    {
        if (!Enum.IsDefined(typeof(UserRole), role)) return false;

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null) return false;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var newUser = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            Role = role, 
            IsMfaEnabled = false
        };

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();
        return true;
    }
    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return new LoginResult { Success = false };

        if (user.IsMfaEnabled)
        {
            return new LoginResult { Success = true, RequiresMfa = true };
        }

        return new LoginResult { Success = true, Token = GenerateJwtToken(user) };
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()), 
            new Claim("UserId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> SetupMfaAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) throw new Exception("User not found");

        var key = KeyGeneration.GenerateRandomKey(20);

        string base32Secret = Base32Encoding.ToString(key);

        user.MfaSecret = base32Secret;
        await _userRepository.SaveChangesAsync();

        return $"otpauth://totp/Aluparts:{email}?secret={base32Secret}&issuer=Aluparts";
    }

    public async Task<bool> VerifyAndEnableMfaAsync(string email, string code)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || string.IsNullOrEmpty(user.MfaSecret)) return false;

        var bytes = Base32Encoding.ToBytes(user.MfaSecret);
        var totp = new Totp(bytes);

        bool isValid = totp.VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));

        if (isValid)
        {
            user.IsMfaEnabled = true;
            await _userRepository.SaveChangesAsync();
        }

        return isValid;
    }

}