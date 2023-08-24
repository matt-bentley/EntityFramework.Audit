using EntityFramework.Audit.Tests.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Weather.Core.Entities;

namespace EntityFramework.Audit.Tests
{
    [TestClass]
    public class RegistrationAuditTests : BaseDbContextTests
    {
        [TestMethod]
        public async Task GivenAuditedDbContext_WhenWhenNotRegistered_ThenNoAudit()
        {
            BuildDbContext(options =>
            {

            });

            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            await Context.SaveChangesAsync();

            forecast = await Context.WeatherForecasts.FirstAsync();
            forecast.Update(2, "Cold");
            await Context.SaveChangesAsync();

            Context.WeatherForecasts.Remove(forecast);
            await Context.SaveChangesAsync();

            Context.Audit.ToList().Should().HaveCount(0);
        }

        [TestMethod]
        public async Task GivenAuditedDbContext_WhenWhenIgnoreProperty_ThenIgnoreOnInsert()
        {
            BuildDbContext(options =>
            {
                options.AuditEntity<WeatherForecast>()
                       .Ignore(e => e.ModifiedDate);
            });

            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            await Context.SaveChangesAsync();           
            
            var auditItem = await Context.Audit.FirstAsync();
            var inserted = JsonSerializer.Deserialize<WeatherForecast>(auditItem.Data);
            inserted.Summary.ToString().Should().Be(forecast.Summary);
            inserted.ModifiedDate.Should().Be(DateTime.MinValue);
        }

        [TestMethod]
        public async Task GivenAuditedDbContext_WhenWhenIgnoreProperty_ThenIgnoreOnUpdate()
        {
            BuildDbContext(options =>
            {
                options.AuditEntity<WeatherForecast>()
                       .Ignore(e => e.ModifiedDate);
            });

            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            await Context.SaveChangesAsync();

            forecast = await Context.WeatherForecasts.FirstAsync();
            forecast.Update(2, "Cold");
            await Context.SaveChangesAsync();

            var auditItem = Context.Audit.ToList().OrderBy(e => e.Time).Last();
            var updates = JsonSerializer.Deserialize<List<UpdateEntry>>(auditItem.Data);
            updates.Should().HaveCount(2);
            updates.FirstOrDefault(e => e.ColumnName == nameof(forecast.ModifiedDate)).Should().BeNull();
        }
    }
}