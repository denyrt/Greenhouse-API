using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Greenhouse.Models.Options
{
    /// <summary>
    /// JWT настройки.
    /// </summary>
    public class JwtOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public int TokenLifeTime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}
