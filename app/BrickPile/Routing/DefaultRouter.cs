using Microsoft.AspNet.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrickPile
{
    /// <summary>
    /// Summary description for DefaultRoute
    /// </summary>
    public class DefaultRoute : IRouter
    {
        public const string ControllerKey = "controller";
        public const string ActionKey = "action";
        public const string CurrentPageKey = "currentPage";
        public const string CurrentModelKey = "currentModel";
        public const string CurrentNodeKey = "currentNode";

        private readonly IRouter _target;
        private readonly IRouteResolver _routeResolver;

        public DefaultRoute(IRouter target, IRouteResolver routeResolver)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (routeResolver == null)
            {
                throw new ArgumentNullException("routeResolver");
            }
            _target = target;
            _routeResolver = routeResolver;
        }

        public string GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {
            // Abort and proceed to other routes in the route table if path contains api or ui
            string[] segments = context.HttpContext.Request.Path.Value.Split(new[] { '/' });
            if (segments.Any(segment => segment.Equals("api", StringComparison.OrdinalIgnoreCase) ||
                                        segment.Equals("ui", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            //ResolveResult result = _routeResolver.Resolve(context.HttpContext.Request.Path.Value);

            var requestPath = context.HttpContext.Request.Path.Value;

            Page currentPage = null;
            dynamic currentModel = null;
            TrieNode node;

            using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenAsyncSession())
            {
                var trie = await session.LoadAsync<Trie>("brickpile/trie");

                if(trie.TryGetNode(requestPath, out node))
                {
                    currentPage = await session.LoadAsync<Page>(node.PageId);
                    currentModel = await session.LoadAsync<dynamic>(node.ContentId);
                }
            }

            if (currentPage != null)
            {
                context.RouteData.Values = new RouteValueDictionary();
                
                context.RouteData.Values[ControllerKey] = currentModel.GetType().Name;
                context.RouteData.Values[ActionKey] = "index";
                context.RouteData.Values[CurrentPageKey] = currentPage;
                context.RouteData.Values[CurrentModelKey] = currentModel;

                context.RouteData.Values[CurrentNodeKey] = node;

                //context.RouteData.Values["area"] = "UI";

                await _target.RouteAsync(context);
            }
        }
    }
}