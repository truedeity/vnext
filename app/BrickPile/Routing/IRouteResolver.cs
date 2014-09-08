using Microsoft.AspNet.Routing;
using System;
using System.Threading.Tasks;

namespace BrickPile
{
    /// <summary>
    /// Summary description for IRouteResolver
    /// </summary>
    public interface IRouteResolver
    {
        Task<ResolveResult> Resolve(RouteContext context);
    }
}