using System.Collections.Generic;

namespace TodoApi.CertAuth
{
    public sealed class CertificateAuthenticationConfig
    {
        public bool Enabled { get; }

        public IReadOnlyList<AuthenticationCertificate> AuthenticationCertificates { get; }

        public CertificateAuthenticationConfig(bool enabled, IReadOnlyList<AuthenticationCertificate> authenticationCertificates)
        {
            Enabled = enabled;
            AuthenticationCertificates = authenticationCertificates;
        }
    }
}