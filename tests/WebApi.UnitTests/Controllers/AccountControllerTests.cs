using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyWarehouse.Infrastructure.Authentication.Model;
using MyWarehouse.Infrastructure.Authentication.Services;
using MyWarehouse.WebApi.Authentication.Models.Dtos;
using MyWarehouse.WebApi.Controllers;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MyWarehouse.WebApi.UnitTests.Controllers
{
    public class AccountControllerTests
    {
        private Mock<IUserService> _mockUserService;
        private AccountController _sut;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>(MockBehavior.Strict);
            _sut = new AccountController(_mockUserService.Object);
        }

        [Test]
        public async Task Login_Successful_ReturnsToken()
        {
            var loginDto = new LoginDto() { Username = "User", Password = "1234" };
            var signInResponse = SignInResult.Success;
            var tokenResponse = new TokenModel("Bearer", "veryFancyToken", DateTime.Now.AddDays(5));
            _mockUserService.Setup(x => x.SignIn(loginDto.Username, loginDto.Password))
                .ReturnsAsync((signInResponse, tokenResponse));
                        
            var result = await _sut.Login(loginDto);
            var resultAlt = await _sut.LoginForm(loginDto);

            result.Should().BeEquivalentTo(resultAlt);
            result.Result.Should().BeAssignableTo(typeof(OkObjectResult));
            var tokenResult = (TokenResponseDto)((OkObjectResult)result.Result).Value;
            tokenResult.token_type.Should().Be(tokenResponse.TokenType);
            tokenResult.access_token.Should().Be(tokenResponse.AccessToken);
            tokenResult.username.Should().Be(tokenResponse.Username);
        }

        [Test]
        public async Task Login_Unsuccessful_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto() { Username = "User", Password = "1234" };
            var signInResponse = SignInResult.Failed;
            _mockUserService.Setup(x => x.SignIn(loginDto.Username, loginDto.Password))
                .ReturnsAsync((signInResponse, null));

            var result = await _sut.Login(loginDto);

            result.Result.Should().BeAssignableTo(typeof(UnauthorizedObjectResult));
        }

        [Test]
        public async Task Login_UserLockedOut_ReturnForbid()
        {
            var loginDto = new LoginDto() { Username = "User", Password = "1234" };
            var signInResponse = SignInResult.LockedOut;
            _mockUserService.Setup(x => x.SignIn(loginDto.Username, loginDto.Password))
                .ReturnsAsync((signInResponse, null));

            var result = await _sut.Login(loginDto);

            result.Result.Should().BeAssignableTo(typeof(ForbidResult));
        }

        [Test]
        public async Task Login_UserNotAllowed_ReturnForbid()
        {
            var loginDto = new LoginDto() { Username = "User", Password = "1234" };
            var signInResponse = SignInResult.NotAllowed;
            _mockUserService.Setup(x => x.SignIn(loginDto.Username, loginDto.Password))
                .ReturnsAsync((signInResponse, null));

            var result = await _sut.Login(loginDto);

            result.Result.Should().BeAssignableTo(typeof(ForbidResult));
        }
    }
}