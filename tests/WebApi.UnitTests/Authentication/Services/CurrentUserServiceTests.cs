using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using MyWarehouse.WebApi.Authentication.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyWarehouse.WebApi.UnitTests.Authentication.Services
{
    public class CurrentUserServiceTests
    {
        [Test]
        public void UserId_WhenUserNotPresent_ReturnsAnonymous()
        {
            var mockCtxAccessor = new Mock<IHttpContextAccessor>();
            mockCtxAccessor.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext());
            var sut = new CurrentUserService(mockCtxAccessor.Object);

            var result = sut.UserId;

            result.Should().Be("Anonymous");
        }

        [Test]
        public void UserId_WhenContextNotPresent_ReturnsSystem()
        {
            var mockCtxAccessor = new Mock<IHttpContextAccessor>();
            var sut = new CurrentUserService(mockCtxAccessor.Object);

            var result = sut.UserId;

            result.Should().Be("System");
        }

        [Test]
        public void UserId_WhenUserPresent_ReturnsUserName()
        {
            var mockCtxAccessor = new Mock<IHttpContextAccessor>();
            mockCtxAccessor.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext() { 
                    User = new ClaimsPrincipal(new[] { 
                        new ClaimsIdentity(new[] { 
                            new Claim(JwtRegisteredClaimNames.UniqueName, 
                            "User123") }) }) });
            var sut = new CurrentUserService(mockCtxAccessor.Object);

            var result = sut.UserId;

            result.Should().Be("User123");
        }
    }
}
