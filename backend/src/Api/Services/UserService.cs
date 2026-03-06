using Api.Data;
using Api.Dtos;
using Api.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email already in use");

            // generate a random salt and hash the password with it
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password + salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = hash,
                Salt = salt,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            // verify password using stored salt
            if (!BCrypt.Net.BCrypt.Verify(password + user.Salt, user.PasswordHash)) return null;

            return user;
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            return _context.Users.FindAsync(id).AsTask();
        }
    }
}