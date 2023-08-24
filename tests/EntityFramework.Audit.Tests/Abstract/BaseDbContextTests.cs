using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Weather.Core.Entities;
using Weather.Infrastructure;

namespace EntityFramework.Audit.Tests.Abstract
{
    public abstract class BaseDbContextTests
    {
        protected const string InMemoryConnectionString = "DataSource=:memory:";
        protected WeatherContext Context;
        protected readonly SqliteConnection Connection;
        protected IServiceCollection Services;

        public BaseDbContextTests()
        {
            Connection = new SqliteConnection(InMemoryConnectionString);
            Connection.Open();
            Services = new ServiceCollection();
        }

        public void CreateContext()
        {
            var options = new DbContextOptionsBuilder<WeatherContext>()
                    .UseSqlite(Connection)
                    .Options;
            var weatherForecastType = typeof(WeatherForecast);
            var auditOptions = new AuditOptions()
            {
                AuditTypes = new Dictionary<Type, PropertyInfo[]>
                {
                    [weatherForecastType] = weatherForecastType.GetProperties()
                }
            };
            Context = new WeatherContext(options, new AuditTracker<WeatherContext>(new DefaultIdentityProvider(), auditOptions));
            Context.Database.EnsureCreated();
        }

        protected async Task ClearAuditAsync()
        {
            var auditItems = await Context.Audit.ToListAsync();
            Context.Audit.RemoveRange(auditItems);
            await Context.SaveChangesAsync();
        }

        protected void BuildDbContext(Action<AuditOptionsBuilder> configure)
        {
            Services.AddDbContext<WeatherContext>(options =>
            {
                options.UseSqlite(Connection);
            });
            Services.AuditDbContext<WeatherContext>(configure);
            var serviceProvider = Services.BuildServiceProvider();
            Context = serviceProvider.GetRequiredService<WeatherContext>();
            Context.Database.EnsureCreated();
        }

        protected void AssertInsertedBy(string user)
        {
            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            Context.SaveChanges();
            var auditItem = Context.Audit.First();
            auditItem.UserName.Should().Be(user);
        }

        [TestCleanup]
        public void Dispose()
        {
            Context?.Dispose();
            Connection.Close();
            Connection.Dispose();
        }
    }
}
