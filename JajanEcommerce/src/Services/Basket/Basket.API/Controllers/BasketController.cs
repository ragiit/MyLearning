using Basket.API.Data;
using Basket.API.Dtos;
using Basket.API.Handler;
using Basket.API.Models;
using Basket.API.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Refit;
using System.Net;
using System.Security.Claims;

namespace Basket.API.Controllers
{
    [Route("api/basket")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class BasketController(ISender mediator, IMenuApi menuApi) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetBasket(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await mediator.Send(new GetBasketQuery(userId), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] List<ShoppingCartItemDto> request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await mediator.Send(new StoreBasketCommand(userId, request), cancellationToken);
            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<bool>> CheckOutBasket(BasketCheckoutRequestDto request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await mediator.Send(new CheckOutBasketCommand(userId, request), cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBasket(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            await mediator.Send(new DeleteBasketCommand(userId), cancellationToken);
            return Ok();
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User ID not found in token");
    }
}