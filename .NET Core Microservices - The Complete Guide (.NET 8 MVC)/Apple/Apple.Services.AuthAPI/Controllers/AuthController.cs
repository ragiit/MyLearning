using Apple.MessageBus;
using Apple.Services.AuthAPI.Models.Dto;
using Apple.Services.AuthAPI.RabbitMQ;
using Apple.Services.AuthAPI.Service.IService;

namespace Apple.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService, IRabbitMQAuthMessageSender messageBus) : ControllerBase
    {
        private readonly ResponseDto _response = new();

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            // message bus publish
            messageBus.SendMessage(model.Email, "register.queue");

            return Ok(_response);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Role))
            {
                _response.IsSuccess = false;
                _response.Message = "Email and Role are required.";
                return BadRequest(_response);
            }
            var result = await authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!result)
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to assign role.";
                return BadRequest(_response);
            }

            _response.Result = "Role assigned successfully.";
            return Ok(_response);
        }
    }
}