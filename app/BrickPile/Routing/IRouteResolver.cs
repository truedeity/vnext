using Microsoft.AspNet.Routing;
using System;
using System.Threading.Tasks;

namespace BrickPile.Routing
{
    /// <summary>
    /// Summary description for IRouteResolver
    /// </summary>
    public interface IRouteResolver
    {
        Task<ResolveResult> Resolve(RouteContext context);
    }
}