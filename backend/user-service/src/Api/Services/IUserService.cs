using Api.Dtos;
using Api.Models;

namespace Api.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterRequest request);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> GetByIdAsync(Guid id);
    }
}