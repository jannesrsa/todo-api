using System.Collections.Generic;

namespace TodoApi.CertAuth
{
    public sealed class CertificateAndRoles
    {
        /// <summary>
        /// The configuration subject that is used to match against the certificate.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The configuration Issuer that is used to match against the certificate.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The configuration Thumbprint that is used to match against the certificate
        /// </summary>
        public string Thumbprint { get; set; }

        /// <summary>
        /// The configured roles that will be associated if the certiifcate matches the expected configuration.
        /// </summary>
        public IReadOnlyList<string> Roles { get; set; }
    }
}