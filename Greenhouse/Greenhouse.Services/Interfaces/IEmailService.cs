using System.Threading.Tasks;

namespace Greenhouse.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendMessageAsync(string email,
                                    string subject,
                                    string content,
                                    string htmlContent = null);
    }
}
