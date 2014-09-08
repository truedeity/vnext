using Microsoft.AspNet.Routing;
using Raven.Client;
using System;
using System.Threading.Tasks;

namespace BrickPile
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
            using (var session = documentStore.OpenAsyncSession())
            {
                return await session.LoadAsync<Trie>("brickpile/trie");
            }
        }
    }
}