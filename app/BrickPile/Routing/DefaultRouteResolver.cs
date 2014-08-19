using System;

namespace BrickPile
{
    /// <summary>
    /// Summary description for DefaultRouteResolver
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly IRouteResolverTrie routeResolverTrie;

        public DefaultRouteResolver(IRouteResolverTrie routeResolverTrie)
	    {
            this.routeResolverTrie = routeResolverTrie;
	    }

        public ResolveResult Resolve(string virtualPath)
        {
            throw new NotImplementedException();
        }
    }
}