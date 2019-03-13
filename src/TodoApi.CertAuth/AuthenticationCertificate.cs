using System;
using System.Collections.Generic;

namespace TodoApi.CertAuth
{
    /// <summary>
    /// Defines the certificate that will act as a security validation role.
    /// </summary>
    public sealed class AuthenticationCertificate
    {
        /// <summary>
        /// The subject of the cert;
        /// <remarks>Used in validation.</remarks>
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// The issuer.
        /// <remarks>Used in validation.</remarks>
        /// </summary>
        public string Issuer { get; }

        /// <summary>
        /// The cert thumbprint/
        /// <remarks>Used in validation.</remarks>
        /// </summary>
        public string Thumbprint { get; }

        /// <summary>
        /// Roles that are bound to security policies.
        /// </summary>
        public IReadOnlyList<string> Roles { get; }

        public AuthenticationCertificate(string subject, string issuer, string thumbprint, IReadOnlyList<string> roles)
        {
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentNullException(nameof(issuer));
            if (string.IsNullOrWhiteSpace(thumbprint)) throw new ArgumentNullException(nameof(thumbprint));

            Subject = subject;
            Issuer = issuer;
            Thumbprint = thumbprint;
            Roles = roles ?? Array.Empty<string>();
        }
    }
}