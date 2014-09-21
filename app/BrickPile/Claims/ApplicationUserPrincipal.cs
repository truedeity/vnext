using System;
using System.Security.Claims;

namespace BrickPile.Claims
{
    /// <summary>
    /// Summary description for ApplicationUserPrincipal
    /// </summary>
    public class ApplicationUserPrincipal : ClaimsPrincipal
    {
        /// <summary>
        /// Gets the name of a BrickPile user.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return FindFirst(ClaimTypes.Name).Value; }
        }
        /// <summary>
        /// Gets the email of a BrickPile user.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email
        {
            get { return FindFirst(ClaimTypes.Email).Value; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AppUserPrincipal" /> class.
        /// </summary>
        /// <param name="principal">The principal.</param>
        public ApplicationUserPrincipal(ClaimsPrincipal principal)
        : base(principal)
        { }
    }
}