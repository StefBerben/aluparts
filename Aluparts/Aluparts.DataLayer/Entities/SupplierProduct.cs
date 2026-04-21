using System.ComponentModel.DataAnnotations;

namespace Aluparts.DataLayer.Entities;

public class SupplierProduct
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string SupplierEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;

    public int StockLevel { get; set; }

    public decimal Price { get; set; }
}