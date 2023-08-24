using EntityFramework.Audit.Tests.Abstract;
using EntityFramework.Audit.Tests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Weather.Core.Entities;

namespace EntityFramework.Audit.Tests
{
    [TestClass]
    public class CustomIdentityProviderAuditTests : BaseDbContextTests
    {
        [TestMethod]
        public void GivenAuditedDbContext_WhenDefaultUser_ThenUseDefaultUser()
        {
            var user = "test@deloitte.com";
            BuildDbContext(options =>
            {
                options.UseDefaultUser(user);
                options.AuditEntity<WeatherForecast>();
            });
            AssertInsertedBy(user);
        }

        [TestMethod]
        public void GivenAuditedDbContext_WhenCustomIdentityProvider_ThenUseUser()
        {
            BuildDbContext(options =>
            {
                options.UseIdentityProvider<IdentityProviderStub>();
                options.AuditEntity<WeatherForecast>();
            });
            AssertInsertedBy(IdentityProviderStub.User);
        }

        [TestMethod]
        public void GivenAuditedDbContext_WhenRegisteredIdentityProvider_ThenUseUser()
        {
            Services.AddSingleton<IAuditIdentityProvider, IdentityProviderStub>();
            BuildDbContext(options =>
            {
                options.UseIdentityProvider<IdentityProviderStub>();
                options.AuditEntity<WeatherForecast>();
            });
            AssertInsertedBy(IdentityProviderStub.User);
        }
    }
}