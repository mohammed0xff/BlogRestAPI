using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.ApiModels;
using Services;
using Microsoft.AspNetCore.Authorization;

namespace BlogApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Sign up a new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationModelRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<string> errorMessages = await _authService.RegisterAsync(model);

            if (errorMessages.Any())
                return BadRequest(errorMessages);

            return Ok();
        }

        /// <summary>
        /// Authenticates the user and returns a jwt token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModelRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);
            
            return Ok(result);
        }

        /// <summary>
        /// Revokes the refresh token
        /// </summary>
        /// <returns></returns>
        [HttpPost("signout")]
        public async Task<IActionResult> Signout()
        {
            var userId = User.Claims.Where(x => x.Type == "uid").FirstOrDefault()?.Value;
            if (userId == null) return BadRequest();
            await _authService.RevokeTokenAsync(userId);
            
            return Ok();
        }

        /// <summary>
        /// refreshes the valid token
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.VerifyAndGenerateToken(tokenRequest);

                if (result == null)
                {
                    return BadRequest(new LoginModelResponse()
                    {
                        IsAuthenticated = false,
                        ErrorMessage = "Invalid token"
                    });
                }

                if (result.ErrorMessage.IsNullOrEmpty())
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest(new LoginModelResponse()
            {
                IsAuthenticated = false,
                ErrorMessage = "Invalid payload"
            });

        }

    }
}
