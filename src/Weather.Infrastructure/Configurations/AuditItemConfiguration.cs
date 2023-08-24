using EntityFramework.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Weather.Infrastructure.Configurations
{
    internal class AuditItemConfiguration : IEntityTypeConfiguration<AuditItem>
    {
        public void Configure(EntityTypeBuilder<AuditItem> builder)
        {
            builder.Property(e => e.User)
                   .HasColumnType("varchar(128)")
                   .IsRequired();
        }
    }
}
