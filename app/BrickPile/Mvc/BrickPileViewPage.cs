using BrickPile.Claims;
using System;
using System.Security.Claims;

namespace BrickPile.Mvc
{
    /// <summary>
    ///     Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax in
    ///     BrickPile UI.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class AppViewPage<TModel> : Microsoft.AspNet.Mvc.Razor.RazorPage<TModel>
    {
        /// <summary>
        ///     Gets the current user.
        /// </summary>
        /// <value>
        ///     The current user.
        /// </value>
        protected ApplicationUserPrincipal CurrentUser
        {
            get { return new ApplicationUserPrincipal(User as ClaimsPrincipal); }
        }
    }

    /// <summary>
    ///     Represents the properties and methods that are needed in order to render a view that uses ASP.NET Razor syntax in
    ///     BrickPile UI.
    /// </summary>
    public abstract class AppViewPage : AppViewPage<dynamic> { }
}