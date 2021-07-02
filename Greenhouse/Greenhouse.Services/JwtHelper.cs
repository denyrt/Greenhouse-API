using Greenhouse.Models.Options;
using Greenhouse.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Greenhouse.Services
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IOptions<JwtOptions> _options;

        public JwtHelper(IOptions<JwtOptions> options)
        {
            _options = options;
        }

        public string CreateJwtToken(params Claim[] claims)
        {
            var securityKey = _options.Value.GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var to = DateTime.Now.AddSeconds(_options.Value.TokenLifeTime);
            var token = new JwtSecurityToken(_options.Value.Issuer,
                _options.Value.Audience,
                claims,
                expires: to,
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
