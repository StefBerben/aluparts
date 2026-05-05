using Aluparts.DataLayer.Entities;
using Aluparts.DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class OrdersController : ControllerBase
{
    private readonly AlupartsDbContext _context;
    public OrdersController(AlupartsDbContext context) => _context = context;

    [HttpGet]
    public IActionResult GetMyOrders()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var orders = _context.ClientOrders.Where(o => o.ClientEmail == email).ToList();
        return Ok(orders);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var order = await _context.ClientOrders.FindAsync(id);

        if (order == null) return NotFound();

        if (order.ClientEmail != email)
        {
            return Forbid("Skill Issue");
        }

        _context.ClientOrders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Order deleted succesfully" });
    }
}