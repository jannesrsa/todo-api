using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TodoApi.CertAuth
{
    internal sealed class CertificateAuthenticationHandler : AuthenticationHandler<CertificateAuthenticationSchemeOptions>
    {
        public CertificateAuthenticationHandler(IOptionsMonitor<CertificateAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var certificate = Context.Connection.ClientCertificate;
            if (certificate != null)
            {
                var roles = GetRolesFromFirstMatchingCertificate(certificate);
                if (roles.Count > 0)
                {
                    var claims = new List<Claim>(roles.Count);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var userIdentity = new ClaimsIdentity(claims, Options.Challenge);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    var ticket = new AuthenticationTicket(userPrincipal, new AuthenticationProperties(), Options.Challenge);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return Task.FromResult(AuthenticateResult.Fail("Authentication Failure"));
        }

        private IReadOnlyList<string> GetRolesFromFirstMatchingCertificate(X509Certificate2 certificate)
        {
            var roles = Options.CertificatesAndRoles
                .Where(r => r.Issuer == certificate.Issuer && r.Subject == certificate.Subject && r.Thumbprint == certificate.Thumbprint)
                .Select(r => r.Roles)
                .FirstOrDefault();

            var coalesce = roles ?? Array.Empty<string>();
            return coalesce;
        }
    }
}