using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace IntegrationTests
{
    public class AuthFlowTests : IAsyncLifetime
    {
        private readonly PostgreSqlTestcontainer _postgres;
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient? _client;

        public AuthFlowTests()
        {
            _postgres = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "testdb",
                    Username = "postgres",
                    Password = "postgres"
                })
                .WithImage("postgres:15-alpine")
                .WithCleanUp(true)
                .Build();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        var dict = new System.Collections.Generic.Dictionary<string, string>
                        {
                            ["ConnectionStrings:DefaultConnection"] = _postgres.ConnectionString
                        };
                        config.AddInMemoryCollection(dict);
                    });
                    builder.ConfigureServices(services =>
                    {
                        // replace db context
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);
                        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_postgres.ConnectionString));
                        // ensure database created
                        var sp = services.BuildServiceProvider();
                        using var scope = sp.CreateScope();
                        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        ctx.Database.EnsureCreated();
                    });
                });
        }

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            await _postgres.DisposeAsync();
            _factory.Dispose();
        }

        [Fact]
        public async Task FullAuthFlow_Works()
        {
            if (_client == null) throw new InvalidOperationException("Client not initialized");

            var registerReq = new RegisterRequest
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@user.com",
                Password = "password123"
            };
            var regResp = await _client.PostAsJsonAsync("/api/auth/register", registerReq);
            regResp.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.Created, regResp.StatusCode);

            var loginReq = new LoginRequest
            {
                Email = registerReq.Email,
                Password = registerReq.Password
            };
            var loginResp = await _client.PostAsJsonAsync("/api/auth/login", loginReq);
            loginResp.EnsureSuccessStatusCode();
            var loginData = await loginResp.Content.ReadFromJsonAsync<System.Collections.Generic.Dictionary<string, string>>();
            Assert.NotNull(loginData);
            Assert.True(loginData.ContainsKey("token"));
            var token = loginData["token"];

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var meResp = await _client.GetAsync("/api/users/me");
            meResp.EnsureSuccessStatusCode();
            var userData = await meResp.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(userData);
            Assert.Equal(registerReq.Email, userData.Email);
            Assert.Equal(registerReq.FirstName, userData.FirstName);
            Assert.Equal(registerReq.LastName, userData.LastName);
        }
    }
}