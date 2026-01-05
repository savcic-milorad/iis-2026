using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TransportSystem.Infrastructure.Identity;

/// <summary>
/// Service for generating JWT tokens
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT token for the given user
    /// </summary>
    public string GenerateToken(ApplicationUser user)
    {
        var jwtSecret = _configuration["JWT:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured");
        var jwtIssuer = _configuration["JWT:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer not configured");
        var jwtAudience = _configuration["JWT:Audience"]
            ?? throw new InvalidOperationException("JWT Audience not configured");
        var expirationHours = int.Parse(_configuration["JWT:ExpirationInHours"] ?? "1");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("FullName", user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Gets the token expiration time based on configuration
    /// </summary>
    public DateTime GetTokenExpirationTime()
    {
        var expirationHours = int.Parse(_configuration["JWT:ExpirationInHours"] ?? "1");
        return DateTime.UtcNow.AddHours(expirationHours);
    }
}
