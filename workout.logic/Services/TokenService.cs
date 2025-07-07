using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces.Services;

namespace workout.logic.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private string _Issuer;

    public TokenService(IConfiguration configuration)
    {

        var secret = configuration["Token:Key"];
        _Issuer = configuration["Token:Issuer"]!;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    }

    private List<Claim> GetClaims(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
        };

        return claims;
    }

    public string CreateToken(User user)
    {

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.Sha256);
        var claims = GetClaims(user);
        var configuration = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Subject = new ClaimsIdentity(claims),
            Issuer = _Issuer,
            Expires = DateTime.Now.AddDays(20)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(configuration);
        return handler.WriteToken(token);
    }
}
