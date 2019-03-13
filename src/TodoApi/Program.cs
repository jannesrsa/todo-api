using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Serilog;
using Serilog.Events;
using System;

#if NET461

using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.Linq;

#endif

namespace TodoApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var pathToContentRoot = AppDomain.CurrentDomain.BaseDirectory;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.RollingFile(Path.Combine(pathToContentRoot, "Logs", "log-{Date}.txt"))
                .CreateLogger();
            try
            {
#if NET461
                var isService = !(Debugger.IsAttached || args.Contains("--console"));

                if (isService)
                {
                    var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                    pathToContentRoot = Path.GetDirectoryName(pathToExe);
                }
#endif

                var host = WebHost.CreateDefaultBuilder(args)
                    .UseContentRoot(pathToContentRoot)
                    .UseStartup<Startup>()
                    .Build();

#if NET461
                if (isService)
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
    }
}