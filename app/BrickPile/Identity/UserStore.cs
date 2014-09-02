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
    public class UserStore<TUser> : IUserStore<TUser>, IUserLoginStore<TUser>, IUserClaimStore<TUser>, IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>, IUserSecurityStampStore<TUser>
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
                    _session.Advanced.DocumentStore.Conventions.RegisterIdConvention<IdentityUser>((dbname, commands, user) => "IdentityUsers/" + user.Id);
                }
                return _session;
            }
        }

        public Task CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Id))
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");

            this.session.StoreAsync(user);

            // This model allows us to lookup a user by name in order to get the id
            var userByName = new IdentityUserByUserName(user.Id, user.UserName);
            this.session.StoreAsync(userByName, IdentityUserByUserName.GetIdentityUserByUserNameId(user.UserName));

            return Task.FromResult(true);
        }

        public Task DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            var userByName = this.session.LoadAsync<IdentityUserByUserName>(IdentityUserByUserName.GetIdentityUserByUserNameId(user.UserName));
            if (userByName != null)
                this.session.Delete(userByName);

            this.session.Delete(user);
            return Task.FromResult(true);
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await this.session.LoadAsync<TUser>(userId);
            return user;
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userByName = await this.session.LoadAsync<IdentityUserByUserName>(IdentityUserByUserName.GetIdentityUserByUserNameId(normalizedUserName));
            if (userByName == null)
                return default(TUser);

            return await FindByIdAsync(userByName.UserId);
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
            throw new NotImplementedException();
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
            this.ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(true);
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
            _disposed = true;
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                var l = new IdentityUserLogin
                {
                    //Id = GetLoginId(login),                   
                    UserId = user.Id,
                    LoginProvider = login.LoginProvider,
                    ProviderKey = login.ProviderKey
                };

                this.session.StoreAsync(l);
                user.Logins.Add(l);
            }

            return Task.FromResult(true);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            string loginId = GetLoginId(login);
            var loginDoc = this.session.LoadAsync<IdentityUserLogin>(loginId);
            if (loginDoc != null)
                this.session.Delete(loginDoc);

            var f = user.Logins.Where(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            foreach(var n in f)
            {
                user.Logins.Remove(n);
            }
           // user.Logins.Remove(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByLoginAsync(UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task AddClaimAsync(TUser user, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(TUser user, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
        #endregion

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
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
    }
    public sealed class IdentityUserByUserName
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public IdentityUserByUserName(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }

        internal static string GetIdentityUserByUserNameId(string userName)
        {
            return string.Format("IdentityUserByUserNames/{0}", userName);
        }
    }
}