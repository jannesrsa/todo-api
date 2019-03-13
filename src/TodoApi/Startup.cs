using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.CertAuth;
using TodoApi.Models;

namespace TodoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Get cert config and configure service for cert auth
            var certConfig = GetCertificateAuthenticationConfig(Configuration);
            if (certConfig.Enabled)
            {
                const string defaultChallengeAndAuthSchemeName = "Certificate";
                var certificateAndRolesCollection = GetCertificateRolesFromConfig(certConfig);

                var authBuilder = services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = defaultChallengeAndAuthSchemeName;
                    options.DefaultChallengeScheme = defaultChallengeAndAuthSchemeName;
                    options.DefaultSignInScheme = defaultChallengeAndAuthSchemeName;
                });

                authBuilder.AddCertificateAuthentication(certOptions =>
                {
                    certOptions.CertificatesAndRoles = certificateAndRolesCollection;
                });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("EnvironmentOwnerPolicy", policy =>
                        policy.RequireRole("EnvironmentOwner"));
                });
            }
        }

        internal static CertificateAuthenticationConfig GetCertificateAuthenticationConfig(IConfiguration configuration)
        {
            var enabled = configuration.GetSection("CertAuthentication")
                .GetValue<bool>("Enabled");
            if (!enabled)
                return new CertificateAuthenticationConfig(false, null);

            var issuer = configuration
                .GetSection("CertAuthentication:EnvironmentOwnerAuthCert")
                .GetValue<string>("Issuer");
            var subject = configuration
                .GetSection($"CertAuthentication:EnvironmentOwnerAuthCert")
                .GetValue<string>("Subject");
            var thumprint = configuration
                .GetSection($"CertAuthentication:EnvironmentOwnerAuthCert")
                .GetValue<string>("Thumbprint");

            // Roles associated to mutual tls cert
            var roles = configuration.GetSection("CertAuthentication:EnvironmentOwnerAuthCert:CertRoles")
                .GetChildren();

            var certRoles = new List<string>();
            foreach (var role in roles)
            {
                certRoles.Add(role.Value);
            }

            var authenticationCertificateList = new List<AuthenticationCertificate> { new AuthenticationCertificate(subject, issuer, thumprint, certRoles) };
            var cfg = new CertificateAuthenticationConfig(true, authenticationCertificateList);
            return cfg;
        }

        internal static HostConfig GetServiceHostConfig(IConfigurationRoot configuration)
        {
            const string sslCertThumbprintConfigPath = "HostSettings:SslCertThumbprint";
            const string runSslConfigPath = "HostSettings:RunSsl";
            const string portConfigPath = "HostSettings:SslPort";

            var runSsl = configuration.GetValue<bool>(runSslConfigPath);
            var sslCertThumbprintString = configuration.GetValue<string>(sslCertThumbprintConfigPath);
            var port = configuration.GetValue<ushort>(portConfigPath);

            var cfg = new HostConfig(runSsl, sslCertThumbprintString, port);
            return cfg;
        }

        private static IReadOnlyList<CertificateAndRoles> GetCertificateRolesFromConfig(CertificateAuthenticationConfig certAuthenticationConfig)
        {
            var certAndRoles = new List<CertificateAndRoles>();

            foreach (var configEntry in certAuthenticationConfig.AuthenticationCertificates)
            {
                certAndRoles.Add(new CertificateAndRoles
                {
                    Thumbprint = configEntry.Thumbprint,
                    Issuer = configEntry.Issuer,
                    Roles = configEntry.Roles,
                    Subject = configEntry.Subject
                });
            }

            return certAndRoles;
        }
    }
}