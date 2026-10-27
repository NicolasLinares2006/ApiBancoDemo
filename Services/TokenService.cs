using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ApiBancoDemo.Services;

public class TokenService
{
    private readonly string _secret;
    private readonly string _issuer;

    public TokenService(IConfiguration config)
    {
        _secret = config["Jwt:Secret"] ?? "CLAVE_SUPER_SECRETA_PARA_CAPACITACION_JMETER_2025_123456";
        _issuer = config["Jwt:Issuer"] ?? "ApiBancoDemo";
    }

    public string GenerarToken(string username, string rol, out DateTime expira)
    {
        expira = DateTime.UtcNow.AddHours(2);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: expira,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public TokenValidationParameters GetValidationParameters()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _issuer,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    }
}