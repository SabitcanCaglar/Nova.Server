using Application.Features.Auths.Dtos;
using Base.Api.Controllers;
using Base.Domain.Entities.Identity;
using IdentityService.Api.Application.Features.Auths.Commands.Login;
using IdentityService.Api.Application.Features.Auths.Commands.Register;
using IdentityService.Api.Application.Models.Login;
using IdentityService.Api.Application.Models.Register;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ApiControllerBase
    {

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            LoginCommand loginCommand = new()
            {
                LoginRequestDto = loginRequestDto,
            };

            var result = await Mediator.Send(loginCommand);
            return Ok(result);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            RegisterCommand registerCommand = new()
            {
                RegisterRequestDto = registerRequestDto,
            };

            RegisteredDto result = await Mediator.Send(registerCommand);
            SetRefreshTokenToCookie(result.RefreshToken);
            return Created("",result.AccessToken);
        }

        private void SetRefreshTokenToCookie(RefreshToken refreshToken)
        {
            CookieOptions cookieOptions = new() { HttpOnly = true ,Expires = DateTime.Now.AddDays(7)};
            Response.Cookies.Append("refreshToken",refreshToken.Token, cookieOptions);
        }
        
    }
}