using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Audit
{
    public interface IAuditTracker<T> where T : DbContext, IAuditDbContext
    {
        void StartTracking(T context);
        void TrackChanges();
    }
}
