using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Serilog;
using Serilog.Events;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Authentication;

#if NET461

using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.Linq;

#endif

namespace TodoApi
{
    public static class Program
    {
        /// <summary>
        /// Load a <see cref="X509Certificate2"/> from My/CurrentUser given its thumbprint.
        /// </summary>
        public static X509Certificate2 LoadCertificate(string thumbprint, bool validOnly)
        {
            if (string.IsNullOrWhiteSpace(thumbprint)) throw new ArgumentNullException(nameof(thumbprint));

            // Remove special characters from thumbprint.
            // If the thumbprint is copied from MMC, it likely contains special characters.
            var sb = new StringBuilder(thumbprint.Length);
            foreach (var c in thumbprint)
            {
                if ((c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    c == '.'
                    || c == '_')
                {
                    sb.Append(c);
                }
            }
            thumbprint = sb.ToString();

            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            try
            {
                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly);

                if (certCollection.Count == 0)
                    throw new ArgumentNullException($@"Cannot find certificate with thumbprint: {thumbprint}.");

                // By convention we return the first certificate
                var certificate = certCollection[0];
                return certificate;
            }
            finally
            {
                certStore.Close();
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                var host = BuildWebHost(args);
#if NET461
                if (!(Debugger.IsAttached || args.Contains("--console")))
                {
                    host.RunAsService();
                    return;
                }
#endif
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var appConfig = configBuilder.Build();
            var serviceConfig = Startup.GetServiceHostConfig(appConfig);

            var webHostBuilder = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            if (serviceConfig.RunSsl)
            {
                webHostBuilder.UseKestrel(serviceConfig, false);
            }
            else
            {
                webHostBuilder.UseKestrel();
            }

            var dir = Directory.GetCurrentDirectory();
            webHostBuilder
                .UseContentRoot(dir)
                .UseSerilog((ctx, cfg) => cfg
                    .ReadFrom.Configuration(ctx.Configuration)
                    .MinimumLevel.Debug());

            var host = webHostBuilder.Build();
            return host;
        }

        /// <summary>
        /// Apply the SSL Certificate and Port details to the <see cref="IWebHostBuilder"/>.
        /// </summary>
        private static void UseKestrel(this IWebHostBuilder webHostBuilder, HostConfig serviceConfig, bool validOnly)
        {
            if (webHostBuilder == null) throw new ArgumentNullException(nameof(webHostBuilder));
            if (serviceConfig == null) throw new ArgumentNullException(nameof(serviceConfig));

            var sslCertificate = LoadCertificate(serviceConfig.SslCertThumbprint, validOnly);

            webHostBuilder.UseKestrel(options =>
            {
                options.Listen(new IPEndPoint(IPAddress.Loopback, serviceConfig.Port), listenOptions =>
                {
                    var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions
                    {
                        ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                        SslProtocols = SslProtocols.Tls12, // Tls 1.0 and 1.1 are broken
                        ServerCertificate = sslCertificate,
                        ClientCertificateValidation = (certificate2, chain, sslPolicyErrors) => true
                    };
                    listenOptions.NoDelay = true;
                    listenOptions.UseHttps(httpsConnectionAdapterOptions).UseConnectionLogging();
                });
            });

            webHostBuilder.UseUrls($"https://*:{serviceConfig.Port}");
        }
    }
}