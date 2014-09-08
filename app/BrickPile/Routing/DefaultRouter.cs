using Microsoft.AspNet.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrickPile
{
    /// <summary>
    /// Summary description for DefaultRoute
    /// </summary>
    public class DefaultRouter : IRouter
    {
        public const string ControllerKey = "controller";
        public const string ActionKey = "action";
        public const string CurrentPageKey = "currentPage";
        public const string CurrentModelKey = "currentModel";
        public const string CurrentNodeKey = "currentNode";
        public const string DefaultAction = "index";

        private readonly IRouter _target;
        private readonly IRouteResolver _routeResolver;

        public DefaultRouter(IRouter target, IRouteResolver routeResolver)
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

            ResolveResult result =  await _routeResolver.Resolve(context);

            if(result == null)
            {
                return;
            }

            //var requestPath = context.HttpContext.Request.Path.Value;

            Page currentPage = null;
            dynamic currentModel = null;

            using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenAsyncSession())
            {
                currentPage = await session.LoadAsync<Page>(result.TrieNode.PageId);
                currentModel = await session.LoadAsync<dynamic>(result.TrieNode.ContentId);
            }

            if (currentPage != null)
            {
                context.RouteData.Values = new RouteValueDictionary();
                
                context.RouteData.Values[ControllerKey] = currentModel.GetType().Name;
                context.RouteData.Values[ActionKey] = result.Action;
                context.RouteData.Values[CurrentPageKey] = currentPage;
                context.RouteData.Values[CurrentModelKey] = currentModel;

                context.RouteData.Values[CurrentNodeKey] = result.TrieNode;

                //context.RouteData.Values["area"] = "UI";

                await _target.RouteAsync(context);
            }
        }
    }
}