using Microsoft.AspNet.Routing;
using Raven.Client;
using System.Threading.Tasks;

namespace BrickPile.Routing.Trie
{
    /// <summary>
    /// Summary description for RouteResolverTrie
    /// </summary>
    public class RouteResolverTrie : IRouteResolverTrie
    {
        private readonly IDocumentStore documentStore;

        public RouteResolverTrie(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
	    }

        public async Task<Trie> LoadTrie(RouteContext context)
        {
            if(context.HttpContext.Items.ContainsKey("brickpile:trie"))
            {
                return context.HttpContext.Items["brickpile:trie"] as Trie;
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                var trie = await session.LoadAsync<Trie>("brickpile/trie");
                context.HttpContext.Items.Add("brickpile:trie", trie);
                return trie;
            }
        }
    }
}