using Microsoft.AspNet.Routing;
using System.Threading.Tasks;
using System.Collections.Generic;
using BrickPile.Models;
using System.Diagnostics;

namespace BrickPile
{
    /// <summary>
    /// Summary description for DefaultRoute
    /// </summary>
    public class DefaultRoute : IRouter
    {
        private readonly IRouter _target;

        public DefaultRoute(IRouter target) {
            _target = target;
        }

        public string GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {
            var requestPath = context.HttpContext.Request.Path.Value;

            using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenAsyncSession())
            {
                var trie = await session.LoadAsync<Trie>("brickpile/trie");

                TrieNode node;
                trie.TryGetValue(requestPath, out node);
            
                if (node != null)
                { 
                    context.RouteData.Values = new Dictionary<string, object>();

                    context.RouteData.Values["controller"] = "Temp";
                    context.RouteData.Values["action"] = "Index";
                    context.RouteData.Values["currentPage"] = await session.LoadAsync<Page>(node.PageId);
                    context.RouteData.Values["currentModel"] = await session.LoadAsync<dynamic>(node.PageId + "/content");

                    //await Task.FromResult(context);

                    await _target.RouteAsync(context);
                }
            }
        }
    }
}