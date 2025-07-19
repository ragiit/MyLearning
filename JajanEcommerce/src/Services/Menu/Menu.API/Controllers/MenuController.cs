using Auth.API.Helper;
using BuildingBlocks;
using Menu.API.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Menu.API.Controllers
{
    [Route("api/menus")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class MenuController(ISender sender) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = $"{Helper.RoleAdmin}")]
        public async Task<ActionResult<ResponseDto<MenuItemDto>>> Create(CreateMenuItemDto dto)
        {
            var result = await sender.Send(new CreateMenuItemCommand(dto));
            return Ok(new ResponseDto<MenuItemDto> { IsSuccess = true, Result = result });
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<MenuItemDto>>>> GetAll([FromQuery] int? pageIndex = 1, [FromQuery] int? pageSize = 10)
        {
            var result = await sender.Send(new GetAllMenuItemsQuery(pageIndex, pageSize));
            return Ok(new ResponseDto<List<MenuItemDto>> { IsSuccess = true, Result = result });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResponseDto<MenuItemDto>>> GetById(Guid id)
        {
            var result = await sender.Send(new GetMenuItemByIdQuery(id));
            if (result is null)
                return NotFound(new ResponseDto<MenuItemDto> { IsSuccess = false, Message = "Menu not found" });

            return Ok(new ResponseDto<MenuItemDto> { IsSuccess = true, Result = result });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{Helper.RoleAdmin}")]
        public async Task<ActionResult<ResponseDto<MenuItemDto>>> Update(Guid id, UpdateMenuItemDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new ResponseDto<MenuItemDto> { IsSuccess = false, Message = "ID mismatch" });

            var result = await sender.Send(new UpdateMenuItemCommand(dto));
            if (result is null)
                return NotFound(new ResponseDto<MenuItemDto> { IsSuccess = false, Message = "Menu not found" });

            return Ok(new ResponseDto<MenuItemDto> { IsSuccess = true, Result = result });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = $"{Helper.RoleAdmin}")]
        public async Task<ActionResult<ResponseDto<bool>>> Delete(Guid id)
        {
            var result = await sender.Send(new DeleteMenuItemCommand(id));
            if (!result)
                return NotFound(new ResponseDto<bool> { IsSuccess = false, Message = "Menu not found" });

            return Ok(new ResponseDto<bool> { IsSuccess = true, Result = true });
        }
    }
}