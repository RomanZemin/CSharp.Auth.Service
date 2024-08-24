using Microsoft.AspNetCore.Mvc;
using Auth.Application.DTOs;
using Auth.WebAPI.Tests.Controllers;
using Moq;

namespace Auth.WebAPI.Tests.Commands
{
    public class AuthControllerSignUpTests : AuthControllerTestBase
    {
        [Fact]
        public async Task SignUpAsync_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new SignUpRequest { Email = "newuser@example.com", Password = "passW@ord123" };
            var response = new AuthenticationResponse { Succeeded = true };
            _mockAuthService.Setup(service => service.SignUpAsync(request))
                            .ReturnsAsync(response);

            // Act
            var result = await _controller.SignUpAsync(request);

            // Assert
            var actionResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(200, actionResult.StatusCode); // Verify the status code is 200
        }

        [Fact]
        public async Task SignUpAsync_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new SignUpRequest { Email = "newuser@example.com", Password = "password123" };
            var response = new AuthenticationResponse
            {
                Succeeded = false,
                Errors = new Dictionary<string, string>
                {
                    { "Email", "Email is already in use" }
                }
            };
            _mockAuthService.Setup(service => service.SignUpAsync(request))
                            .ReturnsAsync(response);

            // Act
            var result = await _controller.SignUpAsync(request);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Errors, actionResult.Value);
        }
    }
}
