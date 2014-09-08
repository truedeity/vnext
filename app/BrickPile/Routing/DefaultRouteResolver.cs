using Microsoft.AspNet.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ResolveResult> Resolve(RouteContext context)
        {
            var trie = await routeResolverTrie.LoadTrie(context);

            // Set the default action to index
            var action = DefaultRouter.DefaultAction;

            if (trie == null || !trie.Any()) return null;

            TrieNode currentNode;

            var segments = context.HttpContext.Request.Path.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // The requested url is for the start page with no action
            // so just return the start page
            if (!segments.Any())
            {
                trie.TryGetNode("/", out currentNode);
            }
            else
            {
                var requestPath = NormalizeRequestPath(context.HttpContext.Request.Path.Value);

                // The normal behaviour is to load the page based on the incoming url
                trie.TryGetNode(requestPath, out currentNode);

                // Try to find the node without the last segment of the url and set the last segment as action
                if (currentNode == null)
                {
                    action = segments.Last();
                    requestPath = string.Join("/", segments, 0, (segments.Length - 1));
                    trie.TryGetNode("/" + requestPath, out currentNode);
                }
            }

            return currentNode == null ? null : new ResolveResult(currentNode, "temp", action);
        }

        private string NormalizeRequestPath(string path)
        {
            return path.TrimEnd(new[] { '/' });
        }
    }
}