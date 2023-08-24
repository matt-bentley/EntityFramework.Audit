
namespace EntityFramework.Audit
{
    public sealed class AuditItem
    {
        public long Id { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string User { get; set; }
    }
}
