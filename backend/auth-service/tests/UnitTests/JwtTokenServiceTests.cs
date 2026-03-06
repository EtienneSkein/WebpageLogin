using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Api.Auth;
using Api.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace UnitTests
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public void GenerateToken_ContainsClaims()
        {
            var options = Options.Create(new JwtOptions
            {
                Secret = "testsecret1234567890abcdefghijklmnopq",
                Issuer = "test",
                Audience = "test",
                ExpMinutes = 60
            });

            var service = new JwtTokenService(options);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "foo@bar.com"
            };

            var token = service.GenerateToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal(user.Id.ToString(), jwt.Subject);
            Assert.Equal(user.Email, jwt.Payload[JwtRegisteredClaimNames.Email]);
            Assert.Equal(options.Value.Issuer, jwt.Issuer);
            Assert.Equal(options.Value.Audience, jwt.Audiences.First());
        }
    }
}