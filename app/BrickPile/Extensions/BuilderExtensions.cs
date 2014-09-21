using BrickPile.FileSystem;
using BrickPile.Routing;
using BrickPile.Routing.Trie;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.FileSystems;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Security.Cookies;
using Microsoft.Framework.DependencyInjection;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.FileSystem;
using System;

namespace BrickPile.Extensions
{
    /// <summary>
    /// Summary description for BuilderExtensions
    /// </summary>
    public static class BuilderExtensions
    {
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

                //services.SetupOptions<MvcOptions>(options =>
                //{
                //    options.ModelBinders.Add(new ModelBinding.TestModelBinder());
                //    options.Filters.Add(typeof(AuthorizeContentAttribute), order: 17);
                //});

                //services.AddSingleton<AuthorizeContentAttribute>();

                services.AddTransient<IFileSystem, RavenDBFileSystem>();

                var builder = services.AddIdentity<ApplicationUser>();
                services.AddSingleton<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
                services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();
                services.AddScoped<IRoleStore<IdentityRole>, RoleStore<IdentityRole>>();
                builder.AddAuthentication()
                .SetupOptions(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonLetterOrDigit = false;
                });
            });

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = ClaimsIdentityOptions.DefaultAuthenticationType,
                LoginPath = new PathString("/ui/auth/login")
            });
           
            // Add MVC to the request pipeline

            app.UseMvc(routes =>
            {
                // Add default route for BrickPile
                routes.Routes.Add(
                    new DefaultRouter(
                        routes.DefaultHandler,
                        new DefaultRouteResolver(DocumentStore,
                            new RouteResolverTrie(DocumentStore))));

                // Add area route
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action}",
                                new { controller = "Dashboard", action = "Index" });

                routes.MapRoute("areaAssetsRoute", "{area:exists}/{controller}/{folder}",
                new { controller = "Assets", folder = "" });

                //routes.MapRoute("areaPagesRoute", "{area:exists}/{controller}/{page}",
                //new { controller = "Pages", action = "Index", page = "" });

                routes.MapRoute("areaAssetsCollectionsRoute", "{area:exists}/{controller}/{action}/{collection}",
                new { controller = "Assets", collection = "" });

                // Add default mvc route
                routes.MapRoute("ActionAsMethod", "{controller}/{action}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            return app;
        }

    }
    //public static class IdentityRavenDBExtensions
    //{
    //    public static IdentityBuilder<TUser> AddIdentityRavenDB<TUser>(this ServiceCollection services)
    //        where TUser : IdentityUser, new()
    //        where TRole : IdentityRole, new()
    //    {
    //        var builder = services.AddIdentity<TUser>();
    //        services.AddSingleton<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
    //        services.AddScoped<UserManager<TUser>, UserManager<TUser>>();
    //        return builder;
    //    }
    //}
}