using Auth.Infrastructure.Identity.Interfaces;
using Auth.Infrastructure.Identity.Models;

namespace Auth.Application.UseCases
{
    public class RefreshTokenUseCase
    {
        private readonly IJWTService _jwtService;

        public RefreshTokenUseCase(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        public string Execute(string refreshToken, ApplicationUser user)
        {
            return _jwtService.RefreshToken(refreshToken, user);
        }
    }
}
