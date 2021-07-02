using System.Security.Claims;

namespace Greenhouse.Services.Interfaces
{
    public interface IJwtHelper
    {
        string CreateJwtToken(params Claim[] claims);
    }
}
