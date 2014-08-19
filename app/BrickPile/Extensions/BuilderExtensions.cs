using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using System;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Security;
using Microsoft.AspNet.Security.Cookies;
using Raven.Abstractions;
using Raven.Client;

using Identity = Microsoft.AspNet.Identity;

namespace BrickPile.Extensions
{
    /// <summary>
    /// Summary description for BuilderExtensions
    /// </summary>
    public static class BuilderExtensions
    {
        private static IBrickPileBootstrapper brickPileBootstrapper;        

        public static IBuilder UseBrickPile(this IBuilder app) {

            //var configuration = new Configuration()
            //            .AddJsonFile("config.json")
            //            .AddEnvironmentVariables();

            /* Error page middleware displays a nice formatted HTML page for any unhandled exceptions in the request pipeline.
             * Note: ErrorPageOptions.ShowAll to be used only at development time. Not recommended for production.
             */
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            app.UseServices(services =>
            {
                // Add MVC services to the services container
                services.AddMvc();
            });

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/ui/auth/login")
            });

            // Add MVC to the request pipeline
            app.UseMvc(routes =>
            {
                // Add default route for BrickPile
                routes.Routes.Add(new DefaultRoute(routes.DefaultHandler));

                // Add area route
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action}",
                                new { controller = "Home", action = "Index" });

                // Add default mvc route
                routes.MapRoute("ActionAsMethod", "{controller}/{action}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            Bootstrap();

            return app;
        }
        private static void Bootstrap()
        {
            // Get the first non-abstract implementation of IBrickPileBootstrapper if one exists in the
            // app domain. If none exist then just use the default one.
            var bootstrapperInterface = typeof(IBrickPileBootstrapper);
            var defaultBootstrapper = typeof(DefaultBrickPileBootstrapper);

            //var locatedBootstrappers =
            //    from asm in AppDomain.CurrentDomain.GetAssemblies() // TODO ignore known assemblies like m$ and such
            //    from type in asm.GetTypes()
            //    where bootstrapperInterface.IsAssignableFrom(type)
            //    where !type.IsInterface
            //    where type != defaultBootstrapper
            //    select type;

            //var bootStrapperType = locatedBootstrappers.FirstOrDefault() ?? defaultBootstrapper;

            var bootStrapperType = defaultBootstrapper;

            brickPileBootstrapper = (IBrickPileBootstrapper)Activator.CreateInstance(bootStrapperType);

            brickPileBootstrapper.Initialise();
        }
    }
}