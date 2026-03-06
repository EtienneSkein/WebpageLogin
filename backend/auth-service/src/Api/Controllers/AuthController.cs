using System;
using System.Threading.Tasks;
using Api.Auth;
using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtTokenService _tokenService;

        public AuthController(IUserService userService, JwtTokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate password requirements
            var (isValid, errors) = PasswordValidator.ValidatePassword(request.Password);
            if (!isValid)
                return BadRequest(new { message = "Password does not meet requirements", errors = errors });

            try
            {
                var user = await _userService.RegisterAsync(request);
                return StatusCode(201);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.AuthenticateAsync(request.Email, request.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                var token = _tokenService.GenerateToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }
    }
}