
namespace EntityFramework.Audit.Tests
{
    [TestClass]
    public class DefaultIdentityProviderAuditTests
    {
        [TestMethod]
        public void GivenDefaultIdentityProvider_WhenDefault_ThenSystem()
        {
            var identityProvider = new DefaultIdentityProvider();
            identityProvider.GetUser().Should().Be("system");
        }

        [TestMethod]
        public void GivenDefaultIdentityProvider_WhenConfigureDefault_ThenConfigured()
        {
            var user = "test@deloitte.com";
            var identityProvider = new DefaultIdentityProvider(user);
            identityProvider.GetUser().Should().Be(user);
        }
    }
}