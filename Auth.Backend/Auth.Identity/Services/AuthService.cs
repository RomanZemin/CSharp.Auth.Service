using Auth.Application.DTOs;
using Auth.Application.Interfaces.Identity;
using Auth.Infrastructure.Identity.Helpers;
using Auth.Infrastructure.Identity.Models;
using Auth.Infrastructure.Identity.Services.JWT;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace Auth.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTService _JWTService;

        private const string SecretKey = "881d43375578e3020726f4d36a5e779d40d3f972e1f3000090567237bca693bb61e4a339df26d38e98561ce5ed8b82f50d4e299a08ee07638c3a197c6c97f7dc";
        private const string Issuer = "WebMonsters";
        private const string Audience = "http://localhost:5250/api";

        public AuthService(IMapper mapper, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _JWTService = new JWTService(SecretKey, Issuer, Audience);
        }

        public async Task<AuthenticationResponse> SignInAsync(SignInRequest request)
        {
            // Perform the sign-in operation
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, false);

            // Retrieve the user information after the sign-in attempt
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
            {
                // Generate JWT token for the user
                user.JWT_Token = _JWTService.GenerateJwtToken(user);
                user.Expires_At = DateTime.UtcNow.AddHours(1).ToString("O");
            }

            // Create a dummy IdentityResult since we don't have one from SignInManager
            var identityResult = IdentityResult.Success; // Assume the identity result is success for now

            // Generate the AuthenticationResponse using the combined method
            return identityResult.ToAuthenticationResponse(signInResult, user);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUserDto?> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }

            string? userId = _userManager.GetUserId(principal);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<ApplicationUserDto>(user);
        }

        public async Task<AuthenticationResponse> SignUpAsync(SignUpRequest request)
        {
            // Create the user
            ApplicationUser user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                Refresh_Token = _JWTService.GenerateRefreshToken()
            };

            // Default IdentityResult as Success to avoid nullability issues
            IdentityResult result = IdentityResult.Success;

            if (request.Password != null)
            {
                // Attempt to create the user
                result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    // Auto sign-in upon successful registration
                    var signInRequest = new SignInRequest
                    {
                        Email = request.Email,
                        Password = request.Password,
                        RememberMe = request.RememberMe.GetValueOrDefault(true) // Default to true if null
                    };

                    AuthenticationResponse signInResult = await SignInAsync(signInRequest);

                    return signInResult;
                }
            }
            // Return result with the error messages if sign-up fails
            return result.ToAuthenticationResponse(null, user);
        }


        public async Task<AuthenticationResponse> ChangePasswordAsync(ClaimsPrincipal principal, string currentPassword, string newPassword)
        {

            ApplicationUser? user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return new AuthenticationResponse
                {
                    Succeeded = false,
                    Errors = new Dictionary<string, string> { { string.Empty, "Invalid request." } }
                };
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result.ToAuthenticationResponse(null, null);
        }

        public async Task<AuthenticationResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.UserEmail);

            if (user == null)
            {
                return new AuthenticationResponse
                {
                    Succeeded = false,
                    Errors = new Dictionary<string, string> { { string.Empty, "Invalid request." } }
                };
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token)), request.NewPassword);

            return result.ToAuthenticationResponse(null, null);
        }

        public async Task<TokenResponse> GeneratePasswordResetTokenAsync(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new TokenResponse
                {
                    Succeeded = false,
                    Errors = new Dictionary<string, string> { { string.Empty, "Invalid request." } }
                };
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new TokenResponse
            {
                Succeeded = true,
                Token = token == null ? string.Empty : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token))
            };
        }

        public async Task<TokenResponse> GenerateEmailConfirmationAsync(ClaimsPrincipal principal)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return new TokenResponse
                {
                    Succeeded = false,
                    Errors = new Dictionary<string, string> { { string.Empty, "Invalid request." } }
                };
            }

            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new TokenResponse
            {
                Succeeded = true,
                UserId = user.Id,
                Token = code == null ? string.Empty : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code))
            };
        }

        public async Task<AuthenticationResponse> ConfirmEmailAsync(EmailConfirmationRequest request)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                return new AuthenticationResponse
                {
                    Succeeded = false,
                    Errors = new Dictionary<string, string> { { string.Empty, "Invalid request." } }
                };
            }

            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            return result.ToAuthenticationResponse(null, null);
        }

        public async Task RefreshSignInAsync(ClaimsPrincipal principal)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(principal);

            if (user != null)
            {
                await _signInManager.RefreshSignInAsync(user);
            }
        }

        public async Task<TokenResponse> GenerateEmailChangeAsync(ClaimsPrincipal principal, string newEmail)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(principal);

            if (user == null)
            {
                return new TokenResponse
                {
                    Succeeded = false,
                    Errors = new Dictionary<string, string> { { string.Empty, "Invalid request." } }
                };
            }

            string token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            return new TokenResponse
            {
                Succeeded = true,
                Token = token == null ? string.Empty : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token)),
                UserId = user.Id
            };
        }
    }
}
