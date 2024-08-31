using Auth.Application.DTOs;
using Auth.Infrastructure.Identity.Models;
using Auth.Infrastructure.Identity.Services.JWT;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Identity.Helpers
{
    public static class IdentityResultExtensions
    {
        public static AuthenticationResponse ToAuthenticationResponse(this IdentityResult identityResult, SignInResult signInResult, ApplicationUser? user)
        {
            // Initialize response based on IdentityResult
            var response = new AuthenticationResponse
            {
                Succeeded = identityResult.Succeeded,
                Errors = identityResult.Succeeded ? null : identityResult.Errors.ToDictionary(e => e.Code, e => e.Description)
            };

            // Check if SignInResult is also provided and use it to update the response
            if (signInResult != null)
            {
                response.Succeeded = signInResult.Succeeded;
                response.Errors = signInResult.Succeeded ? null : new Dictionary<string, string>();

                if (!signInResult.Succeeded)
                {
                    // Populate errors if the sign-in failed
                    if (signInResult.IsLockedOut)
                    {
                        response.Errors.Add("LockedOut", "User account is locked out.");
                    }
                    if (signInResult.IsNotAllowed)
                    {
                        response.Errors.Add("NotAllowed", "User is not allowed to sign in.");
                    }
                    if (signInResult.RequiresTwoFactor)
                    {
                        response.Errors.Add("TwoFactorRequired", "Two-factor authentication is required.");
                    }
                    else
                    {
                        response.Errors.Add("InvalidCredentials", "Invalid email or password.");
                    }
                }
                else if (user != null)
                {
                    // Populate user data and access token if the sign-in is successful
                    response.Data = new UserData
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };

                    // Generate JWT token and expiration date
                    response.Access = new AccessToken
                    {
                        Jwt = JWTService.GenerateJwtToken(user), // Replace with your actual JWT generation logic
                        ExpiresAt = DateTime.UtcNow.AddHours(1).ToString("o") // Example expiration time in ISO 8601 format
                    };
                }
            }

            return response;
        }
    }
}
