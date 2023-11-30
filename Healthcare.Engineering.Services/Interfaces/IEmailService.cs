using System.Threading.Tasks;

namespace Healthcare.Engineering.Services.Interfaces;

public interface IEmailService
{
    Task Send(string email, string message);
}