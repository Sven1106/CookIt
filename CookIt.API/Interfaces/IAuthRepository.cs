using CookIt.API.Models;
using System.Threading.Tasks;

namespace CookIt.API.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> RegisterAsync(User user, string password);
        Task<User> LoginAsync(string email, string password);
        Task<bool> UserExistsAsync(string email);

    }
}
