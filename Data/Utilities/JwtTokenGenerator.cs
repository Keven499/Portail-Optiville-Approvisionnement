using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    private readonly JwtTokenGeneratorOptions _defaultOptions;


    //public JwtTokenGenerator(string secretKey, string issuer, string audience)
    //{
    //    _secretKey = secretKey;
    //    _issuer = issuer;
    //    _audience = audience;
    //}
    public JwtTokenGenerator(IOptions<JwtTokenGeneratorOptions> options)
    {
        _secretKey = options.Value.SecretKey;
        _issuer = options.Value.Issuer;
        _audience = options.Value.Audience;
    }

    public string GenerateToken(string userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            // new Claim(ClaimTypes.Role, role), // pas besoin de role pour le moment
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique ID for the token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(2), // Expiration time
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GetEmailFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        // Récupère le claim pour l'email (par exemple, stocké sous "email" ou "sub")
        return jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "email" || claim.Type == JwtRegisteredClaimNames.Sub)?.Value;
    }


}

public class JwtTokenGeneratorOptions
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}