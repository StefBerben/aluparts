using System.ComponentModel.DataAnnotations;

namespace Aluparts.DataLayer.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public bool IsMfaEnabled { get; set; } = false;
    public string? MfaSecret { get; set; }

    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }

    public enum UserRole
    {
        Client,
        Supplier
    }
}