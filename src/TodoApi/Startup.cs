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

            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
    }
}