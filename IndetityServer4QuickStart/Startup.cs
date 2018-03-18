using IndetityServer4QuickStart.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IndetityServer4QuickStart
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // register IdentityServer in DI
            // Also registers an in-memory store for runtime state.
            // DeveloperSigningCredential creates a temporary ky material for signing tokens.
            // Need to be replaced by some persitent key material for production scenarios.
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Configuration.GetApiResources())
                .AddInMemoryClients(Configuration.GetClients())
                // Adds support for resource owner password grant
                // Adds support to user related services typically used by a login ui
                // adds support for a profile based on the test users
                .AddTestUsers(Configuration.GetUsers());


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

             app.UseIdentityServer();
        }
    }
}
