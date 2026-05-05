using Aluparts.DataLayer.Entities;
using Aluparts.DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Supplier")]
public class ProductsController : ControllerBase
{
    private readonly AlupartsDbContext _context;
    public ProductsController(AlupartsDbContext context) => _context = context;

    [HttpGet]
    public IActionResult GetMyStock()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var stock = _context.SupplierProducts.Where(p => p.SupplierEmail == email).ToList();
        return Ok(stock);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] int newStock)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var product = await _context.SupplierProducts.FindAsync(id);
        if (product == null) return NotFound();

        if (product.SupplierEmail != email)
        {
            return Forbid("You can only update your own supply.");
        }

       
        product.StockLevel = newStock;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Supply updated!" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Supplier")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var product = await _context.SupplierProducts.FindAsync(id);
        if (product == null) return NotFound();
        if (product.SupplierEmail != email)
        {
            return Forbid("You can only update your own supply.");
        }

        _context.SupplierProducts.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Product deleted succesfully." });
    }
}