using System;
using System.Threading.Tasks;
using Api.Data;
using Api.Dtos;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTests
{
    public class UserServiceTests
    {
        private async Task<AppDbContext> GetContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task Register_NewUser_Succeeds()
        {
            var context = await GetContextAsync();
            var service = new UserService(context);
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            };

            var user = await service.RegisterAsync(request);

            Assert.NotNull(user);
            Assert.Equal(request.Email, user.Email);
            Assert.NotEqual(request.Password, user.PasswordHash); // ensure not plaintext
            Assert.False(string.IsNullOrEmpty(user.Salt));
        }

        [Fact]
        public async Task Register_DuplicateEmail_Throws()
        {
            var context = await GetContextAsync();
            var service = new UserService(context);
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            };

            await service.RegisterAsync(request);

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(request));
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsUser()
        {
            var context = await GetContextAsync();
            var service = new UserService(context);
            var request = new RegisterRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Password = "secret"
            };
            var user = await service.RegisterAsync(request);

            var auth = await service.AuthenticateAsync(request.Email, request.Password);
            Assert.NotNull(auth);
            Assert.Equal(user.Email, auth!.Email);
        }

        [Fact]
        public async Task Authenticate_InvalidPassword_ReturnsNull()
        {
            var context = await GetContextAsync();
            var service = new UserService(context);
            var request = new RegisterRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane2@example.com",
                Password = "secret"
            };
            await service.RegisterAsync(request);

            var auth = await service.AuthenticateAsync(request.Email, "wrong");
            Assert.Null(auth);
        }

        [Fact]
        public async Task Register_SamePasswordDifferentSalt_HashesDiffer()
        {
            var context = await GetContextAsync();
            var service = new UserService(context);
            var request1 = new RegisterRequest
            {
                FirstName = "A",
                LastName = "User",
                Email = "a1@example.com",
                Password = "common"
            };
            var request2 = new RegisterRequest
            {
                FirstName = "B",
                LastName = "User",
                Email = "b1@example.com",
                Password = "common"
            };

            var user1 = await service.RegisterAsync(request1);
            var user2 = await service.RegisterAsync(request2);

            Assert.NotEqual(user1.PasswordHash, user2.PasswordHash);
            Assert.NotEqual(user1.Salt, user2.Salt);
        }
    }
}