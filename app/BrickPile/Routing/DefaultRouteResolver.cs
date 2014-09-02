using Microsoft.AspNet.Routing;
using System;

namespace BrickPile
{
    /// <summary>
    /// Summary description for DefaultRouteResolver
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly IRouteResolverTrie routeResolverTrie;
        private readonly RouteContext context;

        public DefaultRouteResolver(RouteContext context, IRouteResolverTrie routeResolverTrie)
	    {
            this.context = context;
            this.routeResolverTrie = routeResolverTrie;
	    }

        public ResolveResult Resolve(string virtualPath)
        {
            throw new NotImplementedException();
        }
    }
}