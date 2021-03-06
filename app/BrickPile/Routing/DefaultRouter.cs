﻿using Microsoft.AspNet.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrickPile.Routing
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

            context.RouteData.Values = new RouteValueDictionary();
                
            context.RouteData.Values[ControllerKey] = "Home";
            context.RouteData.Values[ActionKey] = result.Action;
            context.RouteData.Values[CurrentPageKey] = result.CurrentPage;
            context.RouteData.Values[CurrentModelKey] = result.CurrentModel;

            context.RouteData.Values[CurrentNodeKey] = result.TrieNode;

            //context.RouteData.Values["area"] = "UI";

            await _target.RouteAsync(context);

        }
    }
}