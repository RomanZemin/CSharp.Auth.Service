using Auth.Infrastructure.Identity.Interfaces;
using Auth.Infrastructure.Identity.Models;

namespace Auth.Application.UseCases
{
    public class GenerateTokenUseCase
    {
        private readonly IJWTService _jwtService;

        public GenerateTokenUseCase(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        public string Execute(ApplicationUser user)
        {
            return _jwtService.GenerateJwtToken(user);
        }
    }
}
