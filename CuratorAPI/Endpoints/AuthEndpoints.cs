using CuratorAPI.Dto.Request;
using CuratorAPI.Dto.Response;
using CuratorAPI.Models;
using CuratorAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CuratorAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/register", async (RegisterRequest request, ICuratorService curatorService) =>
            {
                var curator = new Curator
                {
                    Username = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    GroupId = request.GroupId
                };

                bool created = await curatorService.RegisterAsync(curator, request.Password);
                return created ? Results.Ok() : Results.Conflict("User already exists");
            });

            app.MapPost("/login", async (LoginRequest request, ICuratorService curatorService, IConfiguration config) =>
            {
                var curator = await curatorService.AuthenticateAsync(request.Username, request.Password);
                if (curator == null) return Results.Unauthorized();

                var accessTokenExpiration = DateTime.UtcNow.AddMinutes(10);
                var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(30);

                var accessToken = GenerateJwtToken(curator, config, accessTokenExpiration);
                var refreshToken = Guid.NewGuid().ToString();

                await curatorService.SaveRefreshTokenAsync(curator, refreshToken, refreshTokenExpiration);

                return Results.Ok(new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiration = accessTokenExpiration,
                    RefreshTokenExpiration = refreshTokenExpiration
                });
            });

            app.MapPost("/refresh", async (RefreshRequest request, ICuratorService curatorService, IConfiguration config) =>
            {
                var valid = await curatorService.ValidateRefreshTokenAsync(request.Username, request.RefreshToken);
                if (!valid) return Results.Unauthorized();

                var curator = await curatorService.AuthenticateAsync(request.Username, null!); // Получить пользователя по имени
                if (curator == null) return Results.Unauthorized();

                var accessTokenExpiration = DateTime.UtcNow.AddMinutes(10);
                var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(30);

                var accessToken = GenerateJwtToken(curator, config, accessTokenExpiration);
                var newRefreshToken = Guid.NewGuid().ToString();

                await curatorService.RevokeRefreshTokenAsync(request.Username, request.RefreshToken);
                await curatorService.SaveRefreshTokenAsync(curator, newRefreshToken, refreshTokenExpiration);

                return Results.Ok(new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiration = accessTokenExpiration,
                    RefreshTokenExpiration = refreshTokenExpiration
                });
            });
        }

        private static string GenerateJwtToken(Curator curator, IConfiguration config, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, curator.Username),
                new Claim("FirstName", curator.FirstName),
                new Claim("LastName", curator.LastName),
                new Claim("GroupId", curator.GroupId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, curator.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = config["Jwt:Issuer"],
                Audience = config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
