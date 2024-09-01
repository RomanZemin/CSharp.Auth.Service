using Auth.Application.DTOs;
using Auth.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Identity.Helpers
{
    public static class IdentityResultExtensions
    {
        public static AuthenticationResponse ToAuthenticationResponse(this IdentityResult identityResult, SignInResult? signInResult, ApplicationUser? user)
        {
            var response = new AuthenticationResponse
            {
                Succeeded = identityResult.Succeeded,
                Errors = identityResult.Succeeded ? null : identityResult.Errors.ToDictionary(e => e.Code, e => e.Description)
            };

            if (signInResult != null)
            {
                response.Succeeded = signInResult.Succeeded;
                response.Errors = signInResult.Succeeded ? new Dictionary<string, string>() : new Dictionary<string, string>();

                if (!signInResult.Succeeded)
                {
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
                        Refresh_Token = user.Refresh_Token,
                        Jwt = user.JWT_Token, // Replace with your actual JWT generation logic
                        ExpiresAt = user.Expires_At // Example expiration time in ISO 8601 format
                    };
                }
            }

            return response;
        }
    }
}
