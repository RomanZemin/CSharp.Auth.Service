using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Auth.Application.DTOs;
using Auth.WebAPI.Tests.Controllers;
using Moq;

namespace Auth.WebAPI.Tests.Commands
{
    public class AuthControllerSignInTests : AuthControllerTestBase
    {
        [Fact]
        public async Task SignInAsync_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new SignInRequest { Email = "test@example.com", Password = "password" };
            var signInResult = new Mock<Microsoft.AspNetCore.Identity.SignInResult>();
            signInResult.SetupGet(result => result.Succeeded).Returns(true); //эту хуйню надо переписать

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var actionResult = Assert.IsType<OkResult>(result.Result);
        }

        [Fact]
        public async Task SignInAsync_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new SignInRequest { Email = "test@example.com", Password = "wrongpassword" };

            var signInResult = new Mock<Microsoft.AspNetCore.Identity.SignInResult>();
            signInResult.SetupGet(result => result.Succeeded).Returns(true); // и эту хуйню тоже надо переписать

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var actionResult = Assert.IsType<BadRequestResult>(result.Result);
            // No need to check the Value as BadRequestResult doesn't have one.
        }

        [Fact]
        public async Task SignInAsync_ExceptionThrown_ReturnsServerError()
        {
            // Arrange
            var request = new SignInRequest { Email = "test@example.com", Password = "password" };
            _mockAuthService.Setup(service => service.SignInAsync(request))
                            .ThrowsAsync(new System.Exception("Internal error"));

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, actionResult.StatusCode);
        }
    }
}
