
namespace EntityFramework.Audit.Tests.TestDoubles
{
    public class IdentityProviderStub : IAuditIdentityProvider
    {
        public static string User = "test@deloitte.co.uk";

        public string GetUser()
        {
            return User;
        }
    }
}
