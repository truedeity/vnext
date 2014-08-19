using System;

namespace BrickPile
{
    /// <summary>
    /// Summary description for IRouteResolver
    /// </summary>
    public interface IRouteResolver
    {
        ResolveResult Resolve(string virtualPath);
    }
}