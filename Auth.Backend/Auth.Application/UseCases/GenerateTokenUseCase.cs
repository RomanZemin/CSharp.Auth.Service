using Auth.Application.Interfaces.Identity;
using Auth.Domain.Models;

namespace Auth.Application.UseCases
{
    public class GenerateTokenUseCase
    {
        private readonly IJWTService _jwtService;

        public GenerateTokenUseCase(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        public string Execute(IUser user)
        {
            return _jwtService.GenerateJwtToken(user);
        }
    }
}
