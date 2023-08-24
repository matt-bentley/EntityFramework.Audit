using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Audit
{
    public interface IAuditDbContext
    {
        public DbSet<AuditItem> Audit { get; set; }
    }
}