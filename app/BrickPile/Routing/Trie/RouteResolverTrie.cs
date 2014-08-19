using Raven.Client;
using System;

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

        public Trie LoadTrie()
        {
            throw new NotImplementedException();
        }
    }
}