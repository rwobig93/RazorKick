using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services.System;
using Application.Settings.AppSettings;
using Microsoft.IdentityModel.Tokens;

namespace Application.Helpers.Auth;

public static class JwtHelpers
{
    public static readonly JwtSecurityTokenHandler JwtHandler = new();
    public const int JwtTokenValidBeforeSeconds = 3;
    private const int JwtRefreshTokenTimeoutMinutes = 15;
    
    public static byte[] GetJwtSecret(AppConfiguration appConfig)
    {
        return Encoding.ASCII.GetBytes(appConfig.Secret);
    }

    public static string GetJwtIssuer(AppConfiguration appConfig)
    {
        return appConfig.BaseUrl;
    }

    public static string GetJwtAudience(AppConfiguration appConfig)
    {
        return $"{appConfig.ApplicationName} - Users";
    }

    public static JwtSecurityToken GetJwtDecoded(string token)
    {
        return JwtHandler.ReadJwtToken(token);
    }

    public static Guid GetJwtUserId(string token)
    {
        var decodedJwt = GetJwtDecoded(token);
        var userId = decodedJwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        var userIdParsed = Guid.Parse(userId);
        
        return userIdParsed;
    }

    public static DateTime GetJwtValidBeforeTime(IDateTimeService dateTime)
    {
        return dateTime.NowDatabaseTime.AddSeconds(-JwtTokenValidBeforeSeconds);
    }

    public static DateTime GetJwtExpirationTime(IDateTimeService dateTime, AppConfiguration appConfig)
    {
        return dateTime.NowDatabaseTime.AddMinutes(appConfig.TokenExpirationMinutes);
    }

    public static DateTime GetJwtRefreshTokenExpirationTime(IDateTimeService dateTime, AppConfiguration appConfig)
    {
        // Add additional buffer for refresh token to be used
        return dateTime.NowDatabaseTime.AddMinutes(appConfig.TokenExpirationMinutes + JwtRefreshTokenTimeoutMinutes);
    }
    
    public static TokenValidationParameters GetJwtValidationParameters(byte[] jwtSecretKey, string issuer, string audience)
    {
        return new TokenValidationParameters()
        {
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    }

    public static SigningCredentials GetSigningCredentials(AppConfiguration appConfig)
    {
        var secret = GetJwtSecret(appConfig);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    public static string GenerateJwtEncryptedToken(IEnumerable<Claim> claims, IDateTimeService dateTime, AppConfiguration appConfig)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: GetJwtValidBeforeTime(dateTime),
            expires: GetJwtExpirationTime(dateTime, appConfig),
            signingCredentials: GetSigningCredentials(appConfig),
            issuer: GetJwtIssuer(appConfig),
            audience: GetJwtAudience(appConfig));
        
        return JwtHandler.WriteToken(token);
    }

    public static string GenerateJwtRefreshToken(IDateTimeService dateTime, AppConfiguration appConfig, Guid userId)
    {
        // Refresh token should only have the ID as to not allow someone access to anything, extra layer of abstraction
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        
        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: GetJwtValidBeforeTime(dateTime),
            expires: GetJwtRefreshTokenExpirationTime(dateTime, appConfig),
            signingCredentials: GetSigningCredentials(appConfig),
            issuer: GetJwtIssuer(appConfig),
            audience: GetJwtAudience(appConfig));
        
        return JwtHandler.WriteToken(token);
    }

    public static TokenValidationParameters GetJwtValidationParameters(AppConfiguration appConfig)
    {
        return GetJwtValidationParameters(GetJwtSecret(appConfig), GetJwtIssuer(appConfig), GetJwtAudience(appConfig));
    }

    public static ClaimsPrincipal? GetClaimsPrincipalFromToken(string? token, AppConfiguration appConfig)
    {
        var validator = GetJwtValidationParameters(appConfig);

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var claimsPrincipal = JwtHandler.ValidateToken(token, validator, out _);
        return claimsPrincipal;
    }

    public static bool IsJwtValid(string? token, AppConfiguration appConfig)
    {
        var claimsPrincipal = GetClaimsPrincipalFromToken(token, appConfig);
        return claimsPrincipal is not null;
    }
}