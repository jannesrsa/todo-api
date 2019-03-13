using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace TodoApi.CertAuth
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddCertificateAuthentication(this AuthenticationBuilder builder,
            Action<CertificateAuthenticationSchemeOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CertificateAuthenticationSchemeOptions>, CertificateAuthenticationPostConfigureOptions>());
            builder.AddScheme<CertificateAuthenticationSchemeOptions, CertificateAuthenticationHandler>("Certificate", "Certificate Authentication", configureOptions);
            return builder;
        }
    }
}