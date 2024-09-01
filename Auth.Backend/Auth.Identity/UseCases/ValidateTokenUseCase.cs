using Auth.Infrastructure.Identity.Interfaces;

namespace Auth.Application.UseCases
{
    public class ValidateTokenUseCase
    {
        private readonly IJWTService _jwtService;

        public ValidateTokenUseCase(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        public bool Execute(string token)
        {
            return _jwtService.ValidateToken(token);
        }
    }
}
