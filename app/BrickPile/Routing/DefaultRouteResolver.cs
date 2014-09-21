using BrickPile.Routing.Trie;
using Microsoft.AspNet.Routing;
using Raven.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrickPile.Routing
{
    /// <summary>
    /// Summary description for DefaultRouteResolver
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly IDocumentStore documentStore;
        private readonly IRouteResolverTrie routeResolverTrie;

        public DefaultRouteResolver(IDocumentStore documentStore, IRouteResolverTrie routeResolverTrie)
	    {
            this.documentStore = documentStore;
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

            if(currentNode == null)
            {
                return null;
            }

            Page currentPage = null;
            dynamic currentModel = null;

            using (var session = documentStore.OpenAsyncSession())
            {
                currentPage = await session.LoadAsync<Page>(currentNode.PageId);
                currentModel = await session.LoadAsync<dynamic>(currentNode.ContentId);
            }

            return new ResolveResult(currentNode, currentPage, currentModel, currentPage.GetType().Name.Replace("Controller",""), action); // TODO: Resolve controller name using IControllerMapper
        }

        private string NormalizeRequestPath(string path)
        {
            return path.TrimEnd(new[] { '/' });
        }
    }
}