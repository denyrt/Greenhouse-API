using Greenhouse.Data.Models;
using System;
using System.Threading.Tasks;

namespace Greenhouse.Services.Interfaces
{
    public interface IUserHelper
    {
        string GetCurrentUserId();

        Task<User> GetCurrentUserAsync();
    }
}