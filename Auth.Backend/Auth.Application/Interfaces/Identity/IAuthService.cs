using Auth.Application.DTOs;
using System.Security.Claims;

namespace Auth.Application.Interfaces.Identity
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> SignInAsync(SignInRequest signInRequest);
        Task SignOutAsync();
        Task<AuthenticationResponse> SignUpAsync(SignUpRequest signUpRequest);
        Task<AuthenticationResponse> ChangePasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
        Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
        Task<TokenResponse> GeneratePasswordResetTokenAsync(string email);
        Task<ApplicationUserDto?> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<TokenResponse> GenerateEmailConfirmationAsync(ClaimsPrincipal user);
        Task<TokenResponse> GenerateEmailChangeAsync(ClaimsPrincipal user, string newEmail);
        Task<AuthenticationResponse> ConfirmEmailAsync(EmailConfirmationRequest emailConfirmationRequest);
        Task RefreshSignInAsync(ClaimsPrincipal user);
    }
}
