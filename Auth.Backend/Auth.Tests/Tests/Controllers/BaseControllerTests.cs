using Moq;
using Microsoft.Extensions.Logging;
using Auth.WebAPI.Controllers;
using Auth.Application.Interfaces.Identity;

namespace Auth.WebAPI.Tests.Controllers
{
    public class AuthControllerTestBase
    {
        protected readonly Mock<ILogger<AuthController>> _mockLogger;
        protected readonly Mock<IAuthService> _mockAuthService;
        protected readonly Mock<IUserService> _mockUserService;
        protected readonly AuthController _controller;

        public AuthControllerTestBase()
        {
            _mockLogger = new Mock<ILogger<AuthController>>();
            _mockAuthService = new Mock<IAuthService>();
            _mockUserService = new Mock<IUserService>();

            _controller = new AuthController(_mockLogger.Object, _mockAuthService.Object, _mockUserService.Object);
        }
    }
}
