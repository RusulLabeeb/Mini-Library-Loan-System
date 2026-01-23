using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookStore.Infrastructure.Services;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IAuthService authService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 1. Check Header
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        try
        {
            var authHeader = Request.Headers.Authorization.ToString();
            var authHeaderVal = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(authHeader);

            // "Basic" check
            if (!authHeaderVal.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Invalid Scheme");

            var credentialBytes = Convert.FromBase64String(authHeaderVal.Parameter ?? string.Empty);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            
            if (credentials.Length != 2)
                return AuthenticateResult.Fail("Invalid Authorization Header Format");

            var email = credentials[0];
            var password = credentials[1];

            // 2. Validate (Now returns null cleanly if failed)
            var user = await authService.ValidateUserCredentials(email, password); 

            if (user == null)
                return AuthenticateResult.Fail("Invalid Username or Password");

            // 3. Create Identity
            var claims = new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()), 
                new Claim(ClaimTypes.Name, user.Email)
            };
            
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}