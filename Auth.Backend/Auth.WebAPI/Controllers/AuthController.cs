using Microsoft.AspNetCore.Mvc;
using Auth.Application.DTOs;
using Auth.Application.Interfaces.Identity;
using Auth.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Auth.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(
            ILogger<AuthController> logger,
            IAuthService authService,
            IUserService userService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<AuthenticationResponse>> SignInAsync(SignInRequest request)
        {
            try
            {
                // Authenticate user and generate authentication token

                Microsoft.AspNetCore.Identity.SignInResult response = await _authService.SignInAsync(request);

                if (!response.Succeeded)
                {
                    
                    return BadRequest( new { reason = "Invalid login or password" } );
                }

                return Ok( new { message = "Sign in successful" } );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message } );
            }
        }

        [HttpPost("signup")]
        public async Task<ActionResult<AuthenticationResponse>> SignUpAsync(SignUpRequest request)
        {
            try
            {
                // Register new user
                AuthenticationResponse response = await _authService.SignUpAsync(request);

                if (response == null || !response.Succeeded)
                {
                    return BadRequest(response.Errors);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message } );
            }
        }

        [HttpPost("signout")]
        public async Task<ActionResult> SignOutAsync()
        {
            // Log out user
            await _authService.SignOutAsync();

            return NoContent();
        }

        [HttpPost("reset")]
        public async Task<ActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                // Send password reset request for user
                AuthenticationResponse response = await _authService.ResetPasswordAsync(request);

                if (response == null || !response.Succeeded)
                {
                    return BadRequest();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message } );
            }
        }

        [HttpGet("confirm")]
        public async Task<ActionResult> RegisterConfirmationAsync()
        {
            try
            {
                TokenResponse response = await _authService.GenerateEmailConfirmationAsync(User);

                if (response == null || !response.Succeeded)
                {
                    return BadRequest();
                }

                // Send email with token code

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message } );
            }
        }

        [HttpPost("confirm")]
        public async Task<ActionResult> ConfirmEmailAsync(EmailConfirmationRequest request)
        {
            try
            {
                // Confirm email address of user
                await _authService.ConfirmEmailAsync(request);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message } );
            }
        }

        // GET: api/auth/whoami
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new { User.Identity.Name } );
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
