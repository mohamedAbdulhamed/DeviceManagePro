using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeviceApp.Models;
using Microsoft.AspNetCore.Authorization;
using DevicesApp.Core.IConfiguration;
using DevicesApp.Dtos.Requests;
using DevicesApp.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using DevicesApp.Dtos.Responses;

namespace DeviceApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUnitOfWork unitOfWork, ILogger<AccountController> logger) : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AccountController> _logger = logger;

    [HttpGet("GetLogs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetLogs()
    {
        try
        {
            return Ok(await _unitOfWork.ActionLogs.GetAll());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting logs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetUsers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserWithRolesDto
                {
                    Id = user.UserId,
                    Name = user.FirstName + " " + user.LastName,
                    UserName = user.UserName ?? "Undefined",
                    Email = user.Email ?? "Undefined",
                    Roles = roles
                });
            }

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting logs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("GetUserById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserId == id);

            if (user is not null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new UserWithRolesDto
                {
                    Id = user.UserId,
                    Name = user.FirstName + " " + user.LastName,
                    UserName = user.UserName ?? "Undefined",
                    Email = user.Email ?? "Undefined",
                    Roles = roles
                };
                return Ok(userDto);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting logs");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid model state.");

        var user = new ApplicationUser { UserId = Guid.NewGuid(), UserName = model.Username, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded) return Ok("User Created successfully!");

        return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid model state.");

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = await GetAuthClaims(user);

            var token = GetToken(authClaims);

            user.RefreshToken = GenerateRefreshToken();

            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = user.RefreshToken
            });
        }
        return Unauthorized("Invalid username or password.");
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (request == null || request.Token == null || request.RefreshToken == null)
        {
            return BadRequest("Invalid client request.");
        }

        var principal = GetPrincipalFromExpiredToken(request.Token);

        if (principal == null) return BadRequest("Invalid token.");

        var userIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
        
        var user = await _userManager.FindByNameAsync(userIdentifier);

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid client request.");
        }

        var newToken = GetToken(await GetAuthClaims(user));

        user.RefreshToken = GenerateRefreshToken();

        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newToken),
            RefreshToken = user.RefreshToken
        });
    }

    private async Task<List<Claim>> GetAuthClaims(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.UserName ?? "Unknown"),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        return authClaims;
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var jwtKey = _configuration["Jwt:Key"];

        if (jwtKey is null) throw new ArgumentNullException(nameof(jwtKey));

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var expiryMinutes = _configuration["Jwt:ExpiryMinutes"];

        if (expiryMinutes is null) throw new ArgumentNullException(nameof(expiryMinutes));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddMinutes(double.Parse(expiryMinutes)),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtKey = _configuration["Jwt:Key"];

        if (jwtKey is null) throw new ArgumentNullException(nameof(jwtKey));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    [HttpPost("add-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddRole([FromBody] string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded) return Ok("Role added!");

            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return BadRequest("Role already exists.");
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] UserRole model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null) return BadRequest("User not found.");

        if (!await _roleManager.RoleExistsAsync(model.Role)) return BadRequest("Role does not exist.");

        var result = await _userManager.AddToRoleAsync(user, model.Role);

        if (result.Succeeded) return Ok("Role assigned!");

        return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    [HttpGet("DeleteLog/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLog(Guid id)
    {
        try
        {
            if(await _unitOfWork.ActionLogs.Delete(id))
                return NoContent();

            return StatusCode(500, "Couldn't delete the log!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting logs");
            return StatusCode(500, "Internal server error");
        }
    }
}
