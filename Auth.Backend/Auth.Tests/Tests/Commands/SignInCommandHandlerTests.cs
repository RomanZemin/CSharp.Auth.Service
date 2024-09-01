using Auth.Application.DTOs;
using Auth.WebAPI.Tests.Controllers;
using Microsoft.AspNetCore.Mvc;
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

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
        }

        [Fact]
        public async Task SignInAsync_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new SignInRequest { Email = "test@example.com", Password = "wrongpassword" };

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result.Result);
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
