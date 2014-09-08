using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.FileSystem;
using System;
using Raven.Abstractions.FileSystem.Notifications;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Security.Cookies;

namespace BrickPile.Extensions
{
    /// <summary>
    /// Summary description for BuilderExtensions
    /// </summary>
    public static class BuilderExtensions
    {
        //public static Func<IDocumentSession, UserManager<IdentityUser>> UserManagerFactory { get; private set; }

        private static readonly Lazy<IDocumentStore> DocStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "BrickPile"
            }; 
            store.Initialize();
            return store;
        });

        private static readonly Lazy<IFilesStore> FileSystemStore = new Lazy<IFilesStore>(() =>
        {
            var store = new FilesStore
            {
                Url = "http://localhost:8080",                
                DefaultFileSystem = "BrickPile"                
            };            
            store.Initialize();
            return store;
        });

        public static IDocumentStore DocumentStore
        {
            get { return DocStore.Value; }
        }

        public static IFilesStore FilesStore
        {
            get { return FileSystemStore.Value; }
        }

        public static IBuilder UseBrickPile(this IBuilder app) {            

            //var configuration = new Configuration()
            //            .AddJsonFile("config.json")
            //            .AddEnvironmentVariables();

            /* Error page middleware displays a nice formatted HTML page for any unhandled exceptions in the request pipeline.
             * Note: ErrorPageOptions.ShowAll to be used only at development time. Not recommended for production.
             */
            
            app.UseErrorPage(ErrorPageOptions.ShowAll);

            //app.UseWelcomePage();

            app.UseServices(services =>
            {
                // Add MVC services to the services container
                services.AddMvc();
                // Add RavenDB DocumentStore to the services container
                services.AddInstance(DocumentStore);
                // Add RavenDB FilesStore to the services container
                services.AddInstance(FilesStore);

                services.AddSingleton<AuthorizeContentAttribute>();

                //var builder = services.AddIdentity<ApplicationUser>();
                //services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
                //builder.AddAuthentication();
            });

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ClaimsIdentityOptions.DefaultAuthenticationType,
                LoginPath = new PathString("/ui/auth/login")
            });

            

            // configure the user manager
            //UserManagerFactory = (session) =>
            //{
            //    var usermanager = new UserManager<IdentityUser>()
            //    return usermanager;
            //};

            // Add MVC to the request pipeline

            app.UseMvc(routes =>
            {
                // Add default route for BrickPile
                routes.Routes.Add(
                    new DefaultRouter(
                        routes.DefaultHandler,
                        new DefaultRouteResolver(
                            new RouteResolverTrie(DocumentStore))));

                // Add area route
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action}",
                                new { controller = "Dashboard", action = "Index" });

                routes.MapRoute("areaAssetsRoute", "{area:exists}/{controller}/{folder}",
                new { controller = "Assets", folder = "" });

                routes.MapRoute("areaAssetsCollectionsRoute", "{area:exists}/{controller}/{action}/{collection}",
                new { controller = "Assets", collection = "" });

                // Add default mvc route
                routes.MapRoute("ActionAsMethod", "{controller}/{action}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            return app;
        }

    }
}