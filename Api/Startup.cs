using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            // Add authetication services to DI and the authentication middleware to the pipeline.
            // These will :
            //  - Validate the incoming token to make sure it is coming from a trusted issuer
            //  - validate that the token is  valid to be used with this api (aka scope)
            services.AddAuthentication("Bearer") // configures "Bearer" as the default scheme
                // Adds the identityserver access token validation handler for use by authentication services.
                .AddIdentityServerAuthentication(options => 
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api1";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Adds the authentication to the pipeline so authentication will be performed automatically on evey call into the host.
            app.UseAuthentication();
            app.UseMvc();
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!, work here");
            //});
        }
    }
}
