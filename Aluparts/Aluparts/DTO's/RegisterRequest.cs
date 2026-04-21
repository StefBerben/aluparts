using Aluparts.DataLayer.Entities;
using System.ComponentModel.DataAnnotations;
using static Aluparts.DataLayer.Entities.User;

namespace Aluparts.API.Models;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}