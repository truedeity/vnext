using Microsoft.AspNet.Routing;
using System;
using System.Threading.Tasks;

namespace BrickPile.Routing.Trie
{
    /// <summary>
    /// Summary description for IRouteResolverTrie
    /// </summary>
    public interface IRouteResolverTrie
    {
        Task<Trie> LoadTrie(RouteContext context);
    }
}