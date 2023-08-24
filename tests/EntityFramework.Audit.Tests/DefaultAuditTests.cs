using EntityFramework.Audit.Tests.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Weather.Core.Entities;

namespace EntityFramework.Audit.Tests
{
    [TestClass]
    public class DefaultAuditTests : BaseDbContextTests
    {
        public DefaultAuditTests()
        {
            CreateContext();
        }

        [TestMethod]
        public async Task GivenAuditedEntity_WhenInsert_ThenAuditInserted()
        {
            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            await Context.SaveChangesAsync();

            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Should().HaveCount(1);
            var insertedEntry = auditItems.First();
            insertedEntry.EntityId.Should().Be(forecast.Id.ToString());
            insertedEntry.Action.Should().Be(AuditActions.Inserted);
            insertedEntry.User.Should().Be("system");

            var insertedForecast = JsonSerializer.Deserialize<WeatherForecast>(insertedEntry.Data);
            insertedForecast.Id.Should().Be(forecast.Id);
            insertedForecast.Summary.Should().Be(forecast.Summary);
            insertedForecast.TemperatureC.Should().Be(forecast.TemperatureC);
            insertedForecast.ModifiedDate.Should().Be(forecast.ModifiedDate);
        }

        [TestMethod]
        public async Task GivenAuditedEntity_WhenUpdate_ThenAuditUpdated()
        {
            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            await Context.SaveChangesAsync();

            forecast = await Context.WeatherForecasts.FindAsync(forecast.Id);
            forecast.Update(2, "Cold");
            await Context.SaveChangesAsync();

            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Should().HaveCount(2);
            auditItems.All(e => e.EntityId == forecast.Id).Should().BeTrue();
            var updatedEntry = auditItems.Last();
            updatedEntry.Action.Should().Be(AuditActions.Updated);

            var updates = JsonSerializer.Deserialize<List<UpdateEntry>>(updatedEntry.Data);
            updates.Should().HaveCount(3);
            var summaryUpdate = updates.Single(e => e.ColumnName == nameof(forecast.Summary));
            summaryUpdate.OriginalValue.ToString().Should().Be("Warm");
            summaryUpdate.NewValue.ToString().Should().Be("Cold");
        }

        [TestMethod]
        public async Task GivenAuditedEntity_WhenDeleted_ThenAuditDeleted()
        {
            var forecast = WeatherForecast.Create(20, "Warm");
            Context.WeatherForecasts.Add(forecast);
            await Context.SaveChangesAsync();

            forecast = await Context.WeatherForecasts.FindAsync(forecast.Id);
            Context.Remove(forecast);
            await Context.SaveChangesAsync();

            var auditItems = await Context.Audit.ToListAsync();
            auditItems.Should().HaveCount(2);
            auditItems.Last().Action.Should().Be(AuditActions.Deleted);
        }
    }
}