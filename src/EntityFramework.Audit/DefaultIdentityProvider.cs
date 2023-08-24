
namespace EntityFramework.Audit
{
    public sealed class DefaultIdentityProvider : IAuditIdentityProvider
    {
        private readonly string _user;
        private const string SystemUser = "system";

        public DefaultIdentityProvider() : this(SystemUser)
        {
            
        }

        public DefaultIdentityProvider(string user)
        {
            _user = user ?? SystemUser;
        }

        public string GetUser()
        {
            return _user;
        }
    }
}
