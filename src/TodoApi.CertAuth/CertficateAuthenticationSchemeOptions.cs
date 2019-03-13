using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace TodoApi.CertAuth
{
    public sealed class CertificateAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; } = "Certificate";

        public IReadOnlyList<CertificateAndRoles> CertificatesAndRoles { get; set; }
    }
}