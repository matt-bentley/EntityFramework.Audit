
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weather.Core.Entities;

namespace Weather.Infrastructure.Configurations
{
    internal class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
    {
        public void Configure(EntityTypeBuilder<WeatherForecast> builder)
        {
            builder.Property(e => e.Summary)
                   .HasColumnType("varchar(64)")
                   .IsRequired();
        }
    }
}
