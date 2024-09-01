using Auth.Application.Interfaces;
using Auth.Application.Interfaces.Identity;
using Auth.Domain.Models;

namespace Auth.Application.UseCases
{
    public class RefreshTokenUseCase
    {
        private readonly IJWTService _jwtService;

        public RefreshTokenUseCase(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        public string Execute(string refreshToken, IUser user)
        {
            return _jwtService.RefreshToken(refreshToken, user);
        }
    }
}
