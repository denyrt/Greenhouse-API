using Greenhouse.Data.Contexts;
using Greenhouse.Data.Models;
using Greenhouse.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Greenhouse.Services
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;

        public UserHelper(IHttpContextAccessor httpContextAccessor,
            AppDbContext appDbContext) 
        {
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
        }

        public async Task<User> GetCurrentUserAsync() 
        {
            var id = GetCurrentUserId();
            if (id == null) return null;
            return await _appDbContext.Users.FindAsync(id);
        }

        public string GetCurrentUserId()
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims;
            return claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
