using EntityFramework.Audit;
using Microsoft.EntityFrameworkCore;
using Weather.Core.Entities;
using Weather.Core.Entities.Abstract;
using Weather.Infrastructure.Configurations;

namespace Weather.Infrastructure
{
    public sealed class WeatherContext : DbContext, IAuditDbContext
    {
        private readonly IAuditTracker<WeatherContext> _auditTracker;

        public WeatherContext(DbContextOptions<WeatherContext> options,
            IAuditTracker<WeatherContext> auditTracker) : base(options)
        {
            _auditTracker = auditTracker;
            _auditTracker.StartTracking(this);
        }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<AuditItem> Audit { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _auditTracker.TrackChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            _auditTracker.TrackChanges();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherForecastConfiguration).Assembly);

            var entityTypes = modelBuilder.Model
                                             .GetEntityTypes()
                                             .Select(e => e.ClrType)
                                             .Where(e => !e.IsAbstract && e.IsAssignableTo(typeof(Entity)));

            foreach (var type in entityTypes)
            {
                var entityBuilder = modelBuilder.Entity(type);
                entityBuilder.Property(nameof(Entity.Id)).ValueGeneratedNever();
            }
        }
    }
}