using System.Text;
using ApplicationCore;
using Infractructure.EF;
using JWT.Algorithms;
using JWT.Builder;
using KickstarterAPI.Configuration;
using KickstarterAPI.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace KickstarterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager,JwtSettings jwtSettings) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await userManager.FindByNameAsync(dto.UserName);
        if (user == null) 
        {
            return BadRequest();
        }
            
        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (result.Succeeded) 
        {
            return Ok(new {token = CreateToken(user)});
        }
        return BadRequest();
    }
    private string CreateToken(UserEntity user)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            .AddClaim(JwtRegisteredClaimNames.Name, user.UserName)
            .AddClaim(JwtRegisteredClaimNames.Gender, "male")
            .AddClaim(JwtRegisteredClaimNames.Email, user.Email)
            .AddClaim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds())
            .AddClaim(JwtRegisteredClaimNames.Jti, Guid.NewGuid())
            .Audience(jwtSettings.Audience)
            .Issuer(jwtSettings.Issuer)
            .Encode();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingUser = await userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null)
            return BadRequest("Username is already taken.");

        var existingEmail = await userManager.FindByEmailAsync(dto.Email);
        if (existingEmail != null)
            return BadRequest("Email is already in use.");
        
        var user = new UserEntity
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Details = new UserDetails
            {
                CreatedAt = DateTime.UtcNow
            }
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (result.Succeeded)
        {
            return Ok("User registered successfully.");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return BadRequest(ModelState);
    }
}