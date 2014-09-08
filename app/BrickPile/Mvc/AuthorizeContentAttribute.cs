using Microsoft.AspNet.Mvc;
using System;
using System.Threading.Tasks;

namespace BrickPile
{
    /// <summary>
    /// Summary description for AuthorizeContentAttribute
    /// </summary>
    public class AuthorizeContentAttribute : AuthorizationFilterAttribute
    {
        public override async Task OnAuthorizationAsync(AuthorizationContext context)
        {
            throw new NotImplementedException("fucker");
        }
    }
}