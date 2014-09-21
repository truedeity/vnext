using Microsoft.AspNet.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Raven.Client;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace BrickPile
{

    /// <summary>
    /// Summary description for UserStore
    /// </summary>
    public class UserStore<TUser> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>
        where TUser : IdentityUser
    {
        private bool _disposed;
        private readonly IDocumentStore _documentStore;
        private IAsyncDocumentSession _session;

        public UserStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        private IAsyncDocumentSession session
        {
            get
            {
                if (_session == null)
                {
                    _session = _documentStore.OpenAsyncSession();
                    //_session.Advanced.DocumentStore.Conventions.RegisterIdConvention<IdentityUser>((dbname, commands, user) => "IdentityUsers/" + user.Id);
                }
                return _session;
            }
        }   

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if(login == null)
            {
                throw new ArgumentNullException("login");
            }

            if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                var l = new IdentityUserLogin
                {
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                };

                session.StoreAsync(l);                
                user.Logins.Add(l);
            }

            return session.SaveChangesAsync();

        }

        public Task CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
                
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");
            }                

            session.StoreAsync(user);

            // This model allows us to lookup a user by name in order to get the id
            //var userByName = new IdentityUserByUserName(user.Id, user.UserName);
            //this.session.StoreAsync(userByName, IdentityUserByUserName.GetIdentityUserByUserNameId(user.UserName));

            return session.SaveChangesAsync();
        }

        public Task DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            //var userByName = await session.LoadAsync<IdentityUserByUserName>(IdentityUserByUserName.GetIdentityUserByUserNameId(user.UserName));
            //if (userByName != null)
            //    this.session.Delete(userByName);

            session.Delete(user);
            return session.SaveChangesAsync();

            //await Task.FromResult(true);
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if(userId == null)
            {
                throw new ArgumentNullException("userId");
            }
            var user = await session.LoadAsync<TUser>(userId);
            return user;
        }

        public async Task<TUser> FindByLoginAsync(UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            string loginId = GetLoginId(login);
            var loginDoc = await session.Include<IdentityUserLogin>(x => x.UserId).LoadAsync(loginId);
            if(loginDoc == null)
            {
                return null;
            }
            TUser user = null;
            user = await session.LoadAsync<TUser>(loginDoc.UserId);
            return await Task.FromResult(user);
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            //var userByName = await this.session.LoadAsync<IdentityUserByUserName>(IdentityUserByUserName.GetIdentityUserByUserNameId(normalizedUserName));

            var userByName = await session.Query<TUser>().SingleOrDefaultAsync(x => x.NormalizedUserName.Equals(normalizedUserName));
            if (userByName == null) { 
                return await Task.FromResult(default(TUser));
            }

            return await Task.FromResult(userByName);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            // todo: ensure logins loaded
            IList<UserLoginInfo> result = user.Logins
                .Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            return Task.FromResult(result);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.UserName);
        }

        public async Task RemoveLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }

            string loginId = GetLoginId(login);
            var loginDoc = await session.LoadAsync<IdentityUserLogin>(loginId);
            if (loginDoc != null) { 
                session.Delete(loginDoc);
            }

            var f = user.Logins.Where(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            foreach (var n in f)
            {
                user.Logins.Remove(n);
            }

            await Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.NormalizedUserName = userName;

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.UserName = userName;

            return Task.FromResult(0);
        }

        public Task UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }

            return session.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).          
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        // ~UserStore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
        #endregion

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }
            if (!user.Roles.Any(x => x.RoleId == roleName))
            {
                user.Roles.Add(new IdentityUserRole<string>() { RoleId = roleName, UserId = user.Id });
            }
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var roleEntity = user.Roles.SingleOrDefault(ur => ur.RoleId == roleName);
            if (roleEntity != null)
            {
                user.Roles.Remove(roleEntity);
            }
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<IList<string>>(user.Roles.Select(x => x.RoleId).ToList());
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(user.Roles.Any(ur => ur.RoleId == roleName));
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }
            IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public Task AddClaimAsync(TUser user, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }
            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
            {
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = user.Claims.FirstOrDefault(uc => uc.UserId == user.Id && uc.ClaimType == claim.Type && uc.ClaimValue == claim.Value);
            if (entity != null)
            {
                user.Claims.Remove(entity);
            }
            session.StoreAsync(user);

            return session.SaveChangesAsync();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) { 
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        internal static string GetLoginId(UserLoginInfo login)
        {
            using (var sha = new SHA1CryptoServiceProvider())
            {
                byte[] clearBytes = Encoding.UTF8.GetBytes(login.LoginProvider + "|" + login.ProviderKey);
                byte[] hashBytes = sha.ComputeHash(clearBytes);
                return "IdentityUserLogins/" + ToHex(hashBytes);
            }
        }
        internal static string ToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                sb.Append(bytes[i].ToString("x2"));
            return sb.ToString();
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.Email = email;
            session.StoreAsync(user);
            return session.SaveChangesAsync();
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            user.EmailConfirmed = confirmed;
            session.StoreAsync(user);

            return session.SaveChangesAsync();
        }

        public Task<TUser> FindByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            return session.Query<TUser>().SingleOrDefaultAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        //public sealed class IdentityUserByUserName
        //{
        //    public string UserId { get; set; }
        //    public string UserName { get; set; }

        //    public IdentityUserByUserName(string userId, string userName)
        //    {
        //        UserId = userId;
        //        UserName = userName;
        //    }

        //    internal static string GetIdentityUserByUserNameId(string userName)
        //    {
        //        return string.Format("IdentityUserByUserNames/{0}", userName);
        //    }
        //}
    }
}