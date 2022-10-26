using dotnet_rpg.Dtos.User;
using dotnet_rpg.Services.AuthService;
using dotnet_rpg.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {

        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo) {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request) {
            var user = new User() { UserName = request.UserName };
            var response = await _authRepo.Register(user, request.Password);
            if (!response.Success) return BadRequest(response);
            return Ok(response);

        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDto request) {          
            var response = await _authRepo.Login(request.UserName, request.Password);
            if (!response.Success) return BadRequest(response);
            return Ok(response);

        }
    }
}
