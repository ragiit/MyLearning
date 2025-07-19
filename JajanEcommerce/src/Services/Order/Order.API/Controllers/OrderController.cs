using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Domain.Repositories;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController(IOrderRepository orderRepository, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet("my")]
    public async Task<ActionResult<List<Order.Domain.Models.Order>>> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found.");

        var orders = await orderRepository.GetByCustomerIdAsync(userId, cancellationToken);

        return Ok(orders);
    }
}