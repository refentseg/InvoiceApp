using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        private readonly UserManager<User> _userManager;
        public TokenService(UserManager<User> userManager,IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        public async Task<string> GenerateToken(User user)
        {
            var claims = new List<Claim> //user claims who they are or have
            {
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.Name,user.UserName!)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles){
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            string tokenKeyString = _config.GetSection("JWTSettings:TokenKey").Value;

             SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                )
            );
            var creds = new SigningCredentials(tokenKey,SecurityAlgorithms.HmacSha512);

            var tokenOptions = new JwtSecurityToken(
                issuer:null,
                audience:null,
                claims:claims,
                expires:DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        } 
    }
}