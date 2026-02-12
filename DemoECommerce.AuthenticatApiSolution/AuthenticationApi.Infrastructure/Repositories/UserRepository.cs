using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user;
        }
        public async Task<GetUserDTO> GetUser(int UserId)
        {
            var user = await context.Users.FindAsync(UserId);
            return user is not null ? new GetUserDTO
            (
                user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!
            ) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null)
                return new Response
                {
                    flag = false,
                    message = "Email not found."
                };
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password!);
            if(!isPasswordValid)
                return new Response
                {
                    flag = false,
                    message = "Invalid password."
                };
            string token = GenerateToken(getUser);
            return new Response(true, token);

        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name!),
                new(ClaimTypes.Email, user.Email!)
            };

            if(!string.IsNullOrEmpty(user.Role) || !Equals("string",user.Role))
            {
                claims.Add(new(ClaimTypes.Role, user.Role!));
            }

            var token =  new JwtSecurityToken(
                issuer: config.GetSection("Authentication:issuer").Value,
                audience: config.GetSection("Authentication:audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getUser = await GetUserByEmail(appUserDTO.Email);
            if (getUser is not null)
                return new Response
                {
                    flag = false,
                    message = "Email already exists."
                };

            var result = await context.Users.AddAsync(new AppUser
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Address = appUserDTO.Address,
                Role = appUserDTO.Role
            });

            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response
            {
                flag = true,
                message = "User registered successfully."
            } : new Response
            {
                flag = false,
                message = "Failed to register user."
            };
        }
    }
}
