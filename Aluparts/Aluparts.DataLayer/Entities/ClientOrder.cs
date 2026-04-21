namespace Aluparts.DataLayer.Entities;

public class ClientOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClientEmail { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty; 
    public decimal TotalPrice { get; set; }
}